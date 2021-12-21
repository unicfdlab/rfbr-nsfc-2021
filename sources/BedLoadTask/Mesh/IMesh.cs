//---------------------------------------------------------------------------
//                          ПРОЕКТ  "МКЭ"
//                         проектировщик:
//                           Потапов И.И.
//---------------------------------------------------------------------------
//                 кодировка : 25.12.2020 Потапов И.И.
//---------------------------------------------------------------------------
//  !!!  Нет флагов для граничных элементов !!! ++ 
//---------------------------------------------------------------------------
namespace BedLoadTask.Mesh
{
    using BedLoadTask.Mesh.FunForm;
    /// <summary>
    /// Тип КЭ сетки в 1D и 2D
    /// </summary>
    public enum TypeMesh
    {
        Line = 0,
        Triangle,
        Rectangle,
        MixMesh
    }
    /// <summary>
    /// Порядок КЭ сетки на которой определяется функция формы
    /// </summary>
    public enum TypeRangeMesh
    {
        mRange1 = 1,
        mRange2,
        mRange3
    }
    /// <summary>
    /// ОО: IMesh - базистная конечно-элементная сетка с 
    /// </summary>
    public interface IMesh
    {
        /// <summary>
        /// Количество узлов на основном КЭ 
        /// </summary>
        TypeFunForm First { get; }
        /// <summary>
        /// Количество узлов на вспомогательном КЭ 
        /// </summary>
        TypeFunForm Second { get; }
        /// <summary>
        /// Порядок сетки на которой работает функция формы
        /// </summary>
        TypeRangeMesh typeRangeMesh { get; }
        /// <summary>
        /// Тип КЭ сетки в 1D и 2D
        /// </summary>
        TypeMesh typeMesh { get; }
        /// <summary>
        /// Вектор конечных элементов в области
        /// </summary>
        TriElement[] GetAreaElems();
        /// <summary>
        /// Вектор конечных элементов на границе
        /// </summary>
        TwoElement[] GetBoundElems();
        /// <summary>
        /// Массив флагов для граничных конечных элементов 
        /// </summary>
        int[] GetBoundElementsFlags();
        /// <summary>
        /// Массив граничных узловых точек
        /// </summary>
        int[] GetBoundKnots();
        /// <summary>
        /// Массив флагов для граничных узловых точек
        /// </summary>
        int[] GetFlagBoundKnots();
        /// <summary>
        /// Координаты X для узловых точек 
        /// </summary>
        double[] GetCoordsX();
        /// <summary>
        /// Координаты Y для узловых точек 
        /// </summary>
        double[] GetCoordsY();
        /// <summary>
        /// Количество элементов
        /// </summary>
        int CountElement { get; }
        /// <summary>
        /// Количество граничных элементов
        /// </summary>
        int CountBoundElement { get; }
        /// <summary>
        /// Количество узлов
        /// </summary>
        int Count { get; }
        /// <summary>
        /// Количество граничных узлов
        /// </summary>
        int BoundCount { get; }
        /// <summary>
        /// Диапазон координат для узлов сетки
        /// </summary>
        double MaxX { get; }
        double MinX { get; }
        double MaxY { get; }
        double MinY { get; }
        /// <summary>
        /// Получить узлы элемента
        /// </summary>
        /// <param name="i">номер элемента</param>
        TypeFunForm ElementKnots(uint i, ref uint[] knots);
        /// <summary>
        /// Получить узлы граничного элемента
        /// </summary>
        /// <param name="i">номер элемента</param>
        TypeFunForm ElementBoundKnots(uint i, ref uint[] bknot);
        /// <summary>
        /// Получить координаты Х вершин КЭ
        /// </summary>
        /// <param name="i">номер элемента</param>
        /// <returns>Координаты Х вершин КЭ</returns>
        void ElemX(uint i, ref double[] X);
        /// <summary>
        /// Получить координаты Y вершин КЭ
        /// </summary>
        /// <param name="i">номер элемента</param>
        /// <returns>Координаты Y вершин КЭ</returns>
        void ElemY(uint i, ref double[] Y);
        /// <summary>
        /// Получить значения функции связанной с сеткой в вершинах КЭ
        /// </summary>
        /// <param name="i">номер элемента</param>
        /// <returns>значения функции связанной с сеткой в вершинах КЭ</returns>
        void ElemValues(double[] Values, uint i, ref double[] elementValue);
        /// <summary>
        /// Получить максимальную разницу м/д номерами узнов на КЭ
        /// </summary>
        /// <param name="i">номер элемента</param>
        /// <returns>максимальная разница м/д номерами узнов на КЭ</returns>
        uint MaxKnotDecrementForElement(uint i);
        /// <summary>
        ///  Вычисление площади КЭ
        /// </summary>
        /// <param name="x">массив координат елемента Х</param>
        /// <param name="y">массив координат елемента Y</param>
        /// <returns></returns>
        double ElemSquare(double[] x, double[] y);
        /// <summary>
        /// Вычисление площади КЭ
        /// </summary>
        /// <param name="element">номер конечного элемента</param>
        /// <returns></returns>
        double ElemSquare(uint element);
        /// <summary>
        /// Вычисление площади КЭ
        /// </summary>
        double ElemSquare(TriElement element);
        /// <summary>
        /// Вычисление длины граничного КЭ
        /// </summary>
        /// <param name="belement">номер граничного конечного элемента</param>
        /// <returns></returns>
        double BoundElemLength(uint belement);
        /// <summary>
        /// Получить выборку граничных узлов по типу ГУ
        /// </summary>
        /// <param name="i">тип ГУ</param>
        /// <returns></returns>
        uint[] BoundKnotsByType(int id);
        /// <summary>
        /// Получить выборку граничных элементов по типу ГУ
        /// </summary>
        /// <param name="i">тип ГУ</param>
        /// <returns></returns>
        uint[][] BoundElementsByType(int id);
        /// <summary>
        /// Получить тип граничных условий для граничного элемента
        /// </summary>
        /// <param name="elem">граничный элемент</param>
        /// <returns>ID типа граничных условий</returns>
        int GetBoundElementFlag(uint elem);
        /// <summary>
        /// Ширина ленты в глобальной матрице жнсткости
        /// </summary>
        /// <returns></returns>
        uint GetWidthMatrix();
        /// <summary>
        /// Клонирование объекта сетки
        /// </summary>
        IMesh Clone();
        /// <summary>
        /// Тестовая печать КЭ сетки в консоль
        /// </summary>
        void Print();
    }
}