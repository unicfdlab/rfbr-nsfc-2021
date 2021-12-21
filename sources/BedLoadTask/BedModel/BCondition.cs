//---------------------------------------------------------------------------
//                 Реализация библиотеки для моделирования 
//                  гидродинамических и русловых процессов
//---------------------------------------------------------------------------
//                Модуль BLLib для расчета донных деформаций 
//                 (учет движения только влекомых наносов)
//                         проектировщик:
//                           Потапов И.И.
//---------------------------------------------------------------------------
//                 кодировка : 27.12.2019 Потапов И.И.
//---------------------------------------------------------------------------
namespace BedLoadTask.BedModel
{
    using System;
    /// <summary>
    /// Перечисление - типы граничных условий задачи
    /// </summary>
    [Serializable]
    public enum TypeBC
    {
        /// <summary>
        /// Ровное дно
        /// </summary>
        Transit = 0,
        /// <summary>
        /// Первого рода - задан уровень дна
        /// </summary>
        Dirichlet,
        /// <summary>
        /// Второго рода - задан объемный расход наносов
        /// </summary>
        Neumann,
        /// <summary>
        /// Переодические граничные условия
        /// </summary>
        Periodic
    }
    /// <summary>
    /// OO: Класс для задания типа граничных условий
    /// и их значения на входе и выходе области
    /// </summary>
    [Serializable]
    public class BCondition
    {
        /// <summary>
        /// Тип граничных условий на входе в область
        /// </summary>
        public TypeBC Inlet;
        /// <summary>
        /// Тип граничных условий на выходе из области
        /// </summary>
        public TypeBC Outlet;
        /// <summary>
        /// Значение величины (уровень дна/поток) 
        /// на входе в область
        /// </summary>
        public double InletValue;
        /// <summary>
        /// Значение величины (уровень дна/поток) 
        /// на выходе из области
        /// </summary>
        public double OutletValue;
        public BCondition()
        {
            Inlet = TypeBC.Dirichlet;
            Outlet = TypeBC.Neumann;
            InletValue = 0;
            OutletValue = 0;
        }
        public BCondition(TypeBC Inlet, TypeBC Outlet,
                double InletValue, double OutletValue)
        {
            this.Inlet = Inlet;
            this.Outlet = Outlet;
            this.InletValue = InletValue;
            this.OutletValue = OutletValue;
            if ((Inlet == TypeBC.Periodic) ||
                (Outlet == TypeBC.Periodic))
            {
                this.Inlet = TypeBC.Periodic;
                this.Outlet = TypeBC.Periodic;
            }
        }
        public BCondition(BCondition BC)
        {
            this.Inlet = BC.Inlet;
            this.Outlet = BC.Outlet;
            this.InletValue = BC.InletValue;
            this.OutletValue = BC.OutletValue;
        }
    }
}
