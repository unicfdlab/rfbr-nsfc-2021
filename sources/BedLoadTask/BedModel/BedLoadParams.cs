//---------------------------------------------------------------------------
//                 Реализация библиотеки для моделирования 
//                  гидродинамических и русловых процессов
//---------------------------------------------------------------------------
//                Модуль BLLib для расчета донных деформаций 
//                 (учет движения только влекомых наносов)
//         по русловой модели Петрова А.Г. и Потапова И.И. от 2014 г.
//                              Потапов И.И.
//                               27.12.19
//---------------------------------------------------------------------------
namespace BedLoadTask.BedModel
{
    using BedLoadTask.Log;
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;

    [Serializable]
    public enum AvalancheType
    {
        NonAvalanche = 0,
        AvalancheSimple,
        AvalancheQuad_2021
    }
    /// <summary>
    /// ОО: Параметры задачи используемые при 
    /// расчете донных деформаций
    /// </summary>
    [Serializable]
    public class BedLoadParams
    {
        /// <summary>
        /// гравитационная постоянная (м/с/с)
        /// </summary>
        [DisplayName("Гравитационная постоянная (м/с^2)")]
        [Category("Физика")]
        public double g => 9.81;
        /// <summary>
        /// плотность воды, кг/м^3
        /// </summary>
        [DisplayName("Плотность воды (кг/м^3)")]
        [Category("Физика")]
        public double rho_w { get; set; }
        /// <summary>
        /// плотность частиц, кг/м^3
        /// </summary>
        [DisplayName("Плотность частиц (кг/м^3)")]
        [Category("Физика")]
        public double rho_s { get; set; }
        /// <summary>
        /// угол внутреннего трения донных частиц
        /// </summary>
        [DisplayName("угол внутреннего трения")]
        [Category("Физика")]
        public double phi { get; set; }
        /// <summary>
        /// параметр Кармана
        /// </summary>
        [DisplayName("параметр Кармана")]
        [Category("Физика")]
        public double kappa { get; set; }
        /// <summary>
        /// диаметр частиц
        /// </summary>
        [DisplayName("диаметр частиц")]
        [Category("Физика")]
        public double d50 { get; set; }
        /// <summary>
        /// коэффициент лобового столкновения частиц
        /// </summary>
        [DisplayName("коэффициент лобового столкновения")]
        [Category("Физика")]
        public double cx { get; set; }
        /// <summary>
        /// коэффициент пористости дна
        /// </summary>
        [DisplayName("коэффициент пористости дна")]
        [Category("Физика")]
        public double epsilon { get; set; }
        /// <summary>
        /// концентрация частиц в активном слое
        /// </summary>
        [DisplayName("концентрация частиц в активном слое")]
        [Category("Физика")]
        public double f { get; set; }
        /// <summary>
        /// Учет лавинного осыпания дна
        /// </summary>
        [DisplayName("учет лавинного осыпания дна")]
        [Category("Физика")]
        public AvalancheType isAvalanche { get; set; }
        /// <summary>
        /// Расход наносов по механизмам движения донного материала
        /// </summary>
        protected const int transit = 0, zeta = 1, all = 2, press = 3;
        /// <summary>
        /// Значение параметров по умолчанию
        /// </summary>
        public BedLoadParams()
        {
            rho_w = 1000;
            rho_s = 2650;
            phi = 30;
            d50 = 0.0005;
            //d50 = 0.001;
            epsilon = 0.35;
            kappa = 0.25;
            cx = 0.5;
            f = 0.1;
            isAvalanche = AvalancheType.AvalancheSimple;
        }
       
        /// <summary>
        /// Конструктор копирования
        /// </summary>
        /// <param name="p"></param>
        public BedLoadParams(BedLoadParams p)
        {
            SetParams(p);
        }
        /// <summary>
        /// Установка параметров
        /// </summary>
        /// <param name="p"></param>
        public void SetParams(BedLoadParams p)
        {
            rho_w = p.rho_w;
            rho_s = p.rho_s;
            phi = p.phi;
            d50 = p.d50;
            epsilon = p.epsilon;
            kappa = p.kappa;
            cx = p.cx;
            f = p.f;
            isAvalanche = p.isAvalanche;
        }
        /// <summary>
        /// Чтение параметров задачи из файла
        /// </summary>
        /// <param name="file"></param>
        public virtual void Load(StreamReader file)
        {
            NumberFormatInfo formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
            string line;
            string[] sline;
            if (file != null)
                LOG.LogMessage("Файл параметров открыт");
            else
            {
                LOG.LogMessage("Файл параметров не найден");
                return;
            }
            line = file.ReadLine();
            sline = line.Split(' ');
            this.rho_w = double.Parse(sline[1].Trim(), formatter);
            LOG.LogMessage("обработана строка " + line);

            line = file.ReadLine();
            sline = line.Split(' ');
            this.rho_s = double.Parse(sline[1].Trim(), formatter);
            LOG.LogMessage("обработана строка " + line);

            line = file.ReadLine();
            sline = line.Split(' ');
            this.phi = double.Parse(sline[1].Trim(), formatter);
            LOG.LogMessage("обработана строка " + line);

            line = file.ReadLine();
            sline = line.Split(' ');
            this.d50 = double.Parse(sline[1].Trim(), formatter);
            LOG.LogMessage("обработана строка " + line);

            line = file.ReadLine();
            sline = line.Split(' ');
            this.epsilon = double.Parse(sline[1].Trim(), formatter);
            LOG.LogMessage("обработана строка " + line);

            line = file.ReadLine();
            sline = line.Split(' ');
            this.kappa = double.Parse(sline[1].Trim(), formatter);
            LOG.LogMessage("обработана строка " + line);

            line = file.ReadLine();
            sline = line.Split(' ');
            this.cx = double.Parse(sline[1].Trim(), formatter);
            LOG.LogMessage("обработана строка " + line);

            line = file.ReadLine();
            sline = line.Split(' ');
            this.f = double.Parse(sline[1].Trim(), formatter);
            LOG.LogMessage("обработана строка " + line);

            line = file.ReadLine();
            sline = line.Split(' ');
            isAvalanche =  (AvalancheType) int.Parse(sline[1].Trim(), formatter);
            LOG.LogMessage("обработана строка " + line);
        }
        public virtual void InitParamsForMesh(int CountKnots, int CountElement) { }
    }
}
