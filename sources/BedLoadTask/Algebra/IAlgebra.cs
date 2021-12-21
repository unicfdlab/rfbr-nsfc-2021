//---------------------------------------------------------------------------
//               Источник код С++ HAlgebra проект MixTasker
//                              Потапов И.И.
//                        - (F) CopyFree 2002 -
//                              28.05.02
//---------------------------------------------------------------------------
//     Адаптер классов реализующих решение систем алгебраических уравнений
//                              Потапов И.И.
//                               28.07.11
//---------------------------------------------------------------------------
//   Реализация библиотеки методов решения систем алгебраических уравнений
//---------------------------------------------------------------------------
//              кодировка к интерфейсу: 15.02.2021 Потапов И.И. 
//---------------------------------------------------------------------------
namespace BedLoadTask.Algebra
{
    /// <summary>
    /// ОО: Общий интерфейс Алгебры
    /// </summary>
    public interface IAlgebra
    {
        /// <summary>
        /// Название метода
        /// </summary>
        string Name { get; }
        /// <summary>
        /// порядок СЛУ
        /// </summary>
        uint N { get; }
        /// <summary>
        /// <summary>
        /// Очистка матрицы и правой части
        /// </summary>
        void Clear();
        /// <summary>
        /// Сборка ГМЖ
        /// </summary>
        void AddToMatrix(double[][] LMartix, uint[] Adress);
        /// <summary>
        /// // Сборка ГПЧ
        /// </summary>
        void AddToRight(double[] LRight, uint[] Adress);
        /// <summary>
        /// Сборка САУ по строкам (не для всех решателей)
        /// </summary>
        /// <param name="ColElems">Коэффициенты строки системы</param>
        /// <param name="ColAdress">Адреса коэффицентов</param>
        /// <param name="IndexRow">Индекс формируемой строки системы</param>
        /// <param name="Right">Значение правой части строки</param>
        void AddStringSystem(double[] ColElems, uint[] ColAdress, uint IndexRow, double R);
        /// <summary>
        /// Добавление в правую часть
        /// </summary>
        void CopyRight(double[] CRight);
        /// <summary>
        /// Удовлетворение ГУ
        /// </summary>
        void BoundConditions(double[] Conditions, uint[] Adress);
        /// <summary>
        /// Выполнение ГУ
        /// </summary>
        void BoundConditions(double Conditions, uint[] Adress);
        /// <summary>
        /// Операция умножения вектора на матрицу
        /// </summary>
        /// <param name="R">результат</param>
        /// <param name="X">умножаемый вектор</param>
        /// <param name="IsRight">знак операции = +/- 1</param>
        void getResidual(ref double[] R, double[] X, int IsRight = 1);
        /// <summary>
        /// Решение СЛУ
        /// </summary>
        void Solve(ref double[] X);
        /// <summary>
        /// Клонирование объекта
        /// </summary>
        /// <returns></returns>
        IAlgebra Clone();
        ///// <summary>
        ///// Вывод САУ на КОНСОЛЬ
        ///// </summary>
        void Print();
    }
}

