//---------------------------------------------------------------------------
//                 Реализация библиотеки для моделирования 
//                  гидродинамических и русловых процессов
//---------------------------------------------------------------------------
//                Модуль BLLib для расчета донных деформаций 
//                 (учет движения только влекомых наносов)
//                              Потапов И.И.
//                               08.04.21
//---------------------------------------------------------------------------
namespace BedLoadTask.BedModel
{
    using BedLoadTask.Algebra;
    using BedLoadTask.Mesh;
    using BedLoadTask.SaveData;
    using System;
    /// <summary>
    /// Модель движения донного матеиала
    /// </summary>
    [Serializable]
    public enum TypeBLModel
    {
        BLModel_1991,
        BLModel_2010,
        BLModel_2014
    }
    /// <summary>
    /// ОО: Шаблон 2D решателя по расчету донных деформаций
    /// </summary>
    public interface IBedLoadTask
    {
        /// <summary>
        /// Модель влекомого движения донного матеиала
        /// </summary>
        TypeBLModel TBLoadModel { get; }
        /// <summary>
        /// Сетка расчетной области
        /// </summary>
        IMesh Mesh { get; }
        /// <summary>
        /// Решатель для СЛАУ 
        /// </summary>
        IAlgebra Algebra { get; }
        /// <summary>
        /// Текщий уровень дна
        /// </summary>
        double[] CZeta { get; }
        /// <summary>
        /// Установка исходных данных (патерн стратегия)
        /// </summary>
        /// <param name="mesh">Сетка расчетной области</param>
        /// <param name="algebra">Решатель для СЛАУ </param>
        /// <param name="BCBed">граничные условия</param>
        /// <param name="Zeta0">начальный уровень дна</param>
        /// <param name="theta">Параметр схемы по времени</param>
        /// <param name="dt">шаг по времени</param>
        /// <param name="isAvalanche">флаг использования лавинной модели</param>
        void SetTask(IMesh mesh, IAlgebra algebra, BCondition BCBed, TypeBLModel blm, double[] Zeta0, double theta, double dt);
        /// <summary>
        /// Вычисление изменений формы донной поверхности 
        /// на одном шаге по времени по модели 
        /// Петрова А.Г. и Потапова И.И. (2010), 2014
        /// Реализация решателя - методом контрольных объемов,
        /// Патанкар (неявный дискретный аналог ст 40 ф.3.40 - 3.41)
        /// Коэффициенты донной подвижности, определяются 
        /// как среднее гармонические величины         
        /// </summary>
        /// <param name="Zeta">>возвращаемая форма дна на n+1 итерации</param>
        /// <param name="tau">придонное касательное напряжение</param>
        /// <param name="P">придонное давление</param>
        void CalkZetaFDM(ref double[] Zeta, double[] tauX, double[] tauY, double[] P = null);
        /// <summary>
        /// Формирование данных для отрисовки связанных с сеткой IMesh со сороны задачи
        /// </summary>
        /// <param name="sp">контейнер данных</param>
        void AddMeshPolesForGraphics(ISavePoint sp);
    }
}
