//---------------------------------------------------------------------------
//                 Реализация библиотеки для моделирования 
//                  гидродинамических и русловых процессов
//---------------------------------------------------------------------------
//                Модуль BLLib для расчета донных деформаций 
//                 (учет движения только влекомых наносов)
//         по русловой модели Петрова А.Г. и Потапова И.И. от 2014 г.
//                              Потапов И.И.
//                               12.04.21
//---------------------------------------------------------------------------
namespace BedLoadTask.BedModel
{
    using BedLoadTask.Algebra;
    using BedLoadTask.Log;
    using BedLoadTask.Mesh;
    using BedLoadTask.SaveData;
    using System;
    /// <summary>
    /// ОО: Абстрактный класс для 2D задач,
    /// реализует общий интерфейст задач
    /// </summary>
    [Serializable]
    public abstract class ABedLoadTask2D : BedLoadParams, IBedLoadTask
    {
        /// <summary>
        /// Модель влекомого движения донного матеиала
        /// </summary>
        public TypeBLModel TBLoadModel { get => blm; }
        protected TypeBLModel blm;
        /// <summary>
        /// Сетка решателя
        /// </summary>
        public IMesh Mesh { get => mesh; }
        protected IMesh mesh;
        /// <summary>
        /// Алгебра задачи
        /// </summary>
        public IAlgebra Algebra { get => algebra; }
        [NonSerialized]
        protected IAlgebra algebra = null;
        /// <summary>
        /// Для правой части
        /// </summary>
        [NonSerialized]
        protected IAlgebra Ralgebra = null;
        /// <summary>
        /// Текщий уровень дна
        /// </summary>
        public double[] CZeta { get => Zeta0; }
        /// <summary>
        /// массив донных отметок на предыдущем слое по времени
        /// </summary>
        protected double[] Zeta0 = null;
        /// <summary>
        /// массив донных отметок
        /// </summary>
        protected double[] Zeta = null;
        ///// <summary>
        ///// гравитационная постоянная (м/с/с)
        ///// </summary>
        //public double g = 9.81;
        /// <summary>
        /// тангенс угла phi
        /// </summary>
        public double tanphi;
        /// <summary>
        /// критические напряжения на ровном дне
        /// </summary>
        public double tau0 = 0;
        /// <summary>
        /// относительная плотность
        /// </summary>
        public double rho_b;
        /// <summary>
        /// параметр стратификации активного слоя, 
        /// в котором переносятся донные частицы
        /// </summary>
        public double s;
        /// <summary>
        /// коэффициент сухого трения
        /// </summary>
        public double Fa0;
        /// <summary>
        /// константа расхода влекомых наносов
        /// </summary>
        public double G1;
        /// <summary>
        /// <summary>
        /// Флаг отладки
        /// </summary>
        public int debug = 0;
        /// <summary>
        /// Поле сообщений о состоянии задачи
        /// </summary>
        public string Message = "Ok";
        /// <summary>
        /// Параметр схемы по времени
        /// </summary>
        public double theta;
        /// <summary>
        /// текущее время расчета 
        /// </summary>
        public double time = 0;
        /// <summary>
        /// текущая итерация по времени 
        /// </summary>
        public int CountTime = 0;
        /// <summary>
        /// относительная точность при вычислении 
        /// изменения донной поверхности
        /// </summary>
        protected double eZeta = MEM.Error7;
        /// <summary>
        /// шаг по времени
        /// </summary>
        public double dt;
        /// <summary>
        /// множитель для приведения придонного давления к напору
        /// </summary>
        double gamma;
        #region Переменные для работы с КЭ аналогом
        /// <summary>
        /// Узлы КЭ
        /// </summary>
        protected uint[] knots = null;
        /// <summary>
        /// координаты узлов КЭ
        /// </summary>
        protected double[] x = null;
        protected double[] y = null;
        /// <summary>
        /// локальная матрица часть СЛАУ
        /// </summary>
        protected double[][] LaplMatrix = null;
        /// <summary>
        /// локальная матрица часть СЛАУ
        /// </summary>
        protected double[][] RMatrix = null;
        /// <summary>
        /// локальная правая часть СЛАУ
        /// </summary>
        protected double[] LocalRight = null;
        /// <summary>
        /// адреса ГУ
        /// </summary>
        protected uint[] adressBound = null;
        /// <summary>
        /// Вспомогательная правая часть
        /// </summary>
        protected double[] MRight = null;
        #endregion
        #region Краевые условия
        /// <summary>
        /// тип задаваемых ГУ
        /// </summary>
        public BCondition BCBed;
        #endregion
        #region Служебные переменные


        /// <summary>
        ///  косинус гамма - косинус угола между 
        ///  нормалью к дну и вертикальной осью
        /// </summary>
        protected double[] CosGamma = null;
        protected double[] tau = null;
        protected double[] G0 = null;
        protected double[] A = null;
        protected double[] B = null;
        protected double[] C = null;
        protected double[] D = null;
        protected double[] ps = null;
        protected int cu;
        //protected double[] a;
        //protected double[] b;
        protected double dz, dx, dp;
        protected double mtau, chi;
        #endregion
        /// <summary>
        /// Конструктор по умолчанию/тестовый
        /// </summary>
        public ABedLoadTask2D(BedLoadParams p) : base(p)
        {
            InitBedLoad();
        }
        public void ReInitBedLoad(BedLoadParams p)
        {
            SetParams(p);
            InitBedLoad();
        }
        /// <summary>
        /// Расчет постоянных коэффициентов задачи
        /// </summary>
        public void InitBedLoad()
        {
            gamma = 1.0 / (rho_w * g);
            // тангенс угла внешнего откоса
            tanphi = Math.Tan(phi / 180 * Math.PI);
            // сухое трение
            Fa0 = tanphi * (rho_s - rho_w) * g;
            // критические напряжения на ровном дне
            tau0 = 9.0 / 8.0 * kappa * kappa * d50 * Fa0 / cx;
            // константа расхода влекомых наносов
            G1 = 4.0 / (3.0 * kappa * Math.Sqrt(rho_w) * Fa0 * (1 - epsilon));
            // относительная плотность
            rho_b = (rho_s - rho_w) / rho_w;
            // параметр стратификации активного слоя, 
            // в котором переносятся донные частицы
            s = f * rho_b;
        }
        /// <summary>
        /// Установка исходных данных
        /// </summary>
        /// <param name="mesh">Сетка расчетной области</param>
        /// <param name="algebra">Решатель для СЛАУ </param>
        /// <param name="BCBed">граничные условия</param>
        /// <param name="Zeta0">начальный уровень дна</param>
        /// <param name="theta">Параметр схемы по времени</param>
        /// <param name="dt">шаг по времени</param>
        /// <param name="isAvalanche">флаг использования лавинной модели</param>
        public virtual void SetTask(IMesh mesh, IAlgebra algebra, BCondition BCBed, TypeBLModel blm, double[] Zeta0, double theta, double dt)
        {
            //ComplecsMesh m = mesh as ComplecsMesh;
            this.mesh = mesh;
            this.algebra = algebra;
            this.Zeta0 = Zeta0;
            this.BCBed = BCBed;
            this.theta = theta;
            this.dt = dt;
            //получение ширины ленты для алгебры

            if (this.algebra == null)
                this.algebra = new SparseAlgebraCG((uint)mesh.Count);
            if (Ralgebra == null)
                this.Ralgebra = this.algebra.Clone();

            uint Count = (uint)mesh.CountElement;
            // узловые массивы
            Zeta = new double[(uint)mesh.Count];
            ps = new double[Count];
            CosGamma = new double[Count];
            tau = new double[Count];
            A = new double[Count];
            B = new double[Count];
            C = new double[Count];
            D = new double[Count];
            G0 = new double[Count];
        }
        /// <summary>
        /// создание/очистка ЛМЖ и ЛПЧ ...
        /// </summary>
        /// <param name="cu">количество неизвестных</param>
        public virtual void InitLocal(int cu, int cs = 1)
        {
            MEM.Alloc<double>(cu, ref x);
            MEM.Alloc<double>(cu, ref y);
            MEM.Alloc<uint>(cu, ref knots);
            // с учетом степеней свободы
            int Count = cu * cs;
            MEM.AllocClear(Count, ref LocalRight);
            MEM.Alloc2DClear(Count, ref LaplMatrix);
            MEM.Alloc2DClear(Count, ref RMatrix);
        }
        /// <summary>
        /// Формирование данных для отрисовки связанных с сеткой IMesh со сороны задачи
        /// </summary>
        /// <param name="sp">контейнер данных</param>
        public abstract void AddMeshPolesForGraphics(ISavePoint sp);
        public abstract void CalkZetaFDM(ref double[] Zeta, double[] tauX, double[] tauY, double[] P = null);
    }
}
