//---------------------------------------------------------------------------
//                          ПРОЕКТ  "МКЭ"
//                         проектировщик:
//                           Потапов И.И.
//---------------------------------------------------------------------------
//                 кодировка : 25.12.2020 Потапов И.И.
//---------------------------------------------------------------------------
namespace BedLoadTask.Mesh
{
    using BedLoadTask.Mesh.FunForm;
    using System;
    using System.Linq;
    //---------------------------------------------------------------------------
    //  ОО: TriMesh - базистная техузловая конечно-элементная сетка 
    //---------------------------------------------------------------------------
    [Serializable]
    public class TriMesh : IMesh
    {
        /// <summary>
        /// Количество узлов на основном КЭ 
        /// </summary>
        public TypeFunForm First { get => TypeFunForm.Form_2D_Triangle_L1; }
        /// <summary>
        /// Количество узлов на вспомогательном КЭ 
        /// </summary>
        public TypeFunForm Second { get => TypeFunForm.Form_1D_L0; }

        /// <summary>
        /// Порядок сетки на которой работает функция формы
        /// </summary>
        public TypeRangeMesh typeRangeMesh { get => TypeRangeMesh.mRange1; }
        /// <summary>
        /// Тип КЭ сетки: 2D
        /// </summary>
        public TypeMesh typeMesh { get => TypeMesh.Triangle; }
        /// <summary>
        /// Вектор конечных элементов в области
        /// </summary>
        public TriElement[] AreaElems;
        /// <summary>
        /// Вектор конечных элементов на границе
        /// </summary>
        public TwoElement[] BoundElems;
        /// <summary>
        /// Тип граничных условий для граничного элемента
        /// </summary>
        public int[] BoundElementsFlag;
        /// <summary>
        /// Массив граничных узловых точек
        /// </summary>
        public int[] BoundKnots;
        /// <summary>
        /// Массив флагов граничных узловых точек
        /// </summary>
        public int[] BoundKnotsFlag;
        /// <summary>
        /// Координаты X для узловых точек 
        /// </summary>
        public double[] CoordsX;
        /// <summary>
        /// Координаты Y для узловых точек 
        /// </summary>
        public double[] CoordsY;
        public TriMesh() { }
        public TriMesh(TriMesh m)
        {
            AreaElems = new TriElement[m.AreaElems.Length];
            for (int i = 0; i < AreaElems.Length; i++)
                AreaElems[i] = m.AreaElems[i];

            BoundElems = new TwoElement[m.BoundElems.Length];
            BoundElementsFlag = new int[m.BoundElems.Length];
            for (int i = 0; i < BoundElems.Length; i++)
            {
                BoundElems[i] = m.BoundElems[i];
                BoundElementsFlag[i] = m.BoundElementsFlag[i];
            }
            BoundKnots = new int[m.BoundKnots.Length];
            BoundKnotsFlag = new int[m.BoundKnotsFlag.Length];
            for (int i = 0; i < BoundKnots.Length; i++)
            {
                BoundKnots[i] = m.BoundKnots[i];
                BoundKnotsFlag[i] = m.BoundKnotsFlag[i];
            }
            CoordsX = new double[m.CoordsX.Length];
            CoordsY = new double[m.CoordsX.Length];
            for (int i = 0; i < CoordsX.Length; i++)
            {
                CoordsX[i] = m.CoordsX[i];
                CoordsY[i] = m.CoordsY[i];
            }
        }
        /// <summary>
        /// Клонирование объекта сетки
        /// </summary>
        public IMesh Clone()
        {
            return new TriMesh(this);
        }
        /// <summary>
        /// Количество элементов
        /// </summary>
        public int CountElement
        {
            get { return AreaElems == null ? 0 : AreaElems.Length; }
        }
        /// <summary>
        /// Количество граничных элементов
        /// </summary>
        public int CountBoundElement
        {
            get { return BoundElems == null ? 0 : BoundElems.Length; }
        }
        /// <summary>
        /// Количество узлов
        /// </summary>
        public int Count
        {
            get { return CoordsX == null ? 0 : CoordsX.Length; }
        }
        /// <summary>
        /// Количество граничных узлов
        /// </summary>
        public int BoundCount
        {
            get { return BoundKnots == null ? 0 : BoundKnots.Length; }
        }
        public double MaxX
        {
            get { return CoordsX == null ? double.MaxValue : CoordsX.Max(); }
        }
        public double MinX
        {
            get { return CoordsX == null ? double.MaxValue : CoordsX.Min(); }
        }
        public double MaxY
        {
            get { return CoordsY == null ? double.MaxValue : CoordsY.Max(); }
        }
        public double MinY
        {
            get { return CoordsY == null ? double.MaxValue : CoordsY.Min(); }
        }
        /// <summary>
        /// Вектор конечных элементов в области
        /// </summary>
        public TriElement[] GetAreaElems() { return AreaElems; }
        /// <summary>
        /// Вектор конечных элементов на границе
        /// </summary>
        public TwoElement[] GetBoundElems() { return BoundElems; }
        /// <summary>
        /// Массив флагов для граничных конечных элементов 
        /// </summary>
        public int[] GetBoundElementsFlags()
        {
            return BoundElementsFlag;
        }
        /// <summary>
        /// Массив граничных узловых точек
        /// </summary>
        public int[] GetBoundKnots() { return BoundKnots; }
        /// <summary>
        /// Массив флагов для граничных узловых точек
        /// </summary>
        public int[] GetFlagBoundKnots() { return BoundKnotsFlag; }
        /// <summary>
        /// Координаты X для узловых точек 
        /// </summary>
        public double[] GetCoordsX() { return CoordsX; }
        /// <summary>
        /// Координаты Y для узловых точек 
        /// </summary>
        public double[] GetCoordsY() { return CoordsY; }
        /// <summary>
        /// Получить узлы элемента
        /// </summary>
        /// <param name="i">номер элемента</param>
        public TriElement Element(uint i)
        {
            return AreaElems[i];
        }
        /// <summary>
        /// Получить узлы элемента
        /// </summary>
        /// <param name="i">номер элемента</param>
        public TypeFunForm ElementKnots(uint i, ref uint[] knots)
        {
            knots[0] = AreaElems[i].Vertex1;
            knots[1] = AreaElems[i].Vertex2;
            knots[2] = AreaElems[i].Vertex3;
            return TypeFunForm.Form_2D_Triangle_L1;
        }
        /// <summary>
        /// Получить узлы граничного элемента
        /// </summary>
        /// <param name="i">номер элемента</param>
        public TypeFunForm ElementBoundKnots(uint i, ref uint[] bknot)
        {
            bknot[0] = BoundElems[i].Vertex1;
            bknot[1] = BoundElems[i].Vertex2;
            return TypeFunForm.Form_1D_L1;
        }
        /// <summary>
        /// Получить координаты Х вершин КЭ
        /// </summary>
        /// <param name="i">номер элемента</param>
        /// <returns>Координаты Х вершин КЭ</returns>
        public void ElemX(uint i, ref double[] X)
        {
            X[0] = CoordsX[AreaElems[i].Vertex1];
            X[1] = CoordsX[AreaElems[i].Vertex2];
            X[2] = CoordsX[AreaElems[i].Vertex3];
        }
        /// <summary>
        /// Получить координаты Y вершин КЭ
        /// </summary>
        /// <param name="i">номер элемента</param>
        /// <returns>Координаты Y вершин КЭ</returns>
        public void ElemY(uint i, ref double[] Y)
        {
            Y[0] = CoordsY[AreaElems[i].Vertex1];
            Y[1] = CoordsY[AreaElems[i].Vertex2];
            Y[2] = CoordsY[AreaElems[i].Vertex3];
        }
        /// <summary>
        /// Получить значения функции связанной с сеткой в вершинах КЭ
        /// </summary>
        /// <param name="i">номер элемента</param>
        /// <returns>значения функции связанной с сеткой в вершинах КЭ</returns>
        public void ElemValues(double[] Values, uint i, ref double[] elementValue)
        {
            elementValue[0] = Values[AreaElems[i].Vertex1];
            elementValue[1] = Values[AreaElems[i].Vertex2];
            elementValue[2] = Values[AreaElems[i].Vertex3];
        }
        public uint MaxKnotDecrementForElement(uint i)
        {
            uint min = AreaElems[i].Vertex1;
            uint max = AreaElems[i].Vertex1;
            min = Math.Min(Math.Min(min, AreaElems[i].Vertex1),
                  Math.Min(AreaElems[i].Vertex2, AreaElems[i].Vertex3));

            max = Math.Max(Math.Max(max, AreaElems[i].Vertex1),
                Math.Max(AreaElems[i].Vertex2, AreaElems[i].Vertex3));
            return (max - min);
        }
        /// <summary>
        ///  Вычисление площади КЭ
        /// </summary>
        /// <param name="x">массив координат елемента Х</param>
        /// <param name="y">массив координат елемента Y</param>
        /// <returns></returns>
        public double ElemSquare(double[] x, double[] y)
        {
            double S = (x[0] * (y[1] - y[2]) + x[1] * (y[2] - y[0]) + x[2] * (y[0] - y[1])) / 2.0;
            return S;
        }
        /// <summary>
        /// Вычисление площади КЭ
        /// </summary>
        public double ElemSquare(uint elem)
        {
            TriElement knot = Element(elem);
            double S = (CoordsX[knot.Vertex1] * (CoordsY[knot.Vertex2] - CoordsY[knot.Vertex3]) +
                        CoordsX[knot.Vertex2] * (CoordsY[knot.Vertex3] - CoordsY[knot.Vertex1]) +
                        CoordsX[knot.Vertex3] * (CoordsY[knot.Vertex1] - CoordsY[knot.Vertex2])) / 2.0;
            return S;
        }
        /// <summary>
        /// Вычисление площади КЭ
        /// </summary>
        public double ElemSquare(TriElement element)
        {
            return (CoordsX[element.Vertex1] * (CoordsY[element.Vertex2] - CoordsY[element.Vertex3]) +
                    CoordsX[element.Vertex2] * (CoordsY[element.Vertex3] - CoordsY[element.Vertex1]) +
                    CoordsX[element.Vertex3] * (CoordsY[element.Vertex1] - CoordsY[element.Vertex2])) / 2.0;
        }
        /// <summary>
        /// Вычисление длины граничного КЭ
        /// </summary>
        /// <param name="belement">номер граничного конечного элемента</param>
        /// <returns></returns>
        public double BoundElemLength(uint belement)
        {
            TwoElement knot = BoundElems[belement];
            double a = CoordsX[knot.Vertex1] - CoordsX[knot.Vertex2];
            double b = CoordsY[knot.Vertex1] - CoordsY[knot.Vertex2];
            double Length = Math.Sqrt(a * a + b * b);
            return Length;
        }
        /// <summary>
        /// Получить выборку граничных узлов по типу ГУ
        /// </summary>
        /// <param name="i">тип ГУ</param>
        /// <returns></returns>
        public uint[] BoundKnotsByType(int i)
        {
            int count = 0;
            for (int k = 0; k < BoundKnots.Length; k++)
            {
                if (BoundKnotsFlag[k] == i)
                    ++count;
            }
            uint[] mass = new uint[count];

            int j = 0;
            for (int k = 0; k < BoundKnots.Length; k++)
            {
                if (BoundKnotsFlag[k] == i)
                    mass[j++] = (uint)BoundKnots[k];
            }
            Array.Sort(mass);
            return mass;
        }
        /// <summary>
        /// Получить выборку граничных элементов по типу ГУ
        /// </summary>
        /// <param name="id">тип ГУ</param>
        /// <returns>массив ГЭ</returns>
        public uint[][] BoundElementsByType(int id)
        {
            int count = 0;
            for (int k = 0; k < CountBoundElement; k++)
            {
                if (BoundElementsFlag[k] == id)
                    ++count;
            }
            uint[][] mass = new uint[count][];
            int j = 0;
            for (int k = 0; k < CountBoundElement; k++)
            {
                if (BoundElementsFlag[k] == id)
                {
                    TwoElement el = BoundElems[k];
                    mass[j] = new uint[2];
                    mass[j][0] = el.Vertex1;
                    mass[j][1] = el.Vertex2;
                    j++;
                }
            }
            return mass;
        }
        /// <summary>
        /// Получить тип граничных условий для граничного элемента
        /// </summary>
        /// <param name="elem">граничный элемент</param>
        /// <returns>ID типа граничных условий</returns>
        public int GetBoundElementFlag(uint elem)
        {
            return BoundElementsFlag[elem];
        }

        /// <summary>
        /// Ширина ленты в глобальной матрице жнсткости
        /// </summary>
        /// <returns></returns>
        public uint GetWidthMatrix()
        {
            uint max = MaxKnotDecrementForElement(0);
            for (uint i = 1; i < AreaElems.Length; i++)
            {
                uint tmp = MaxKnotDecrementForElement(i);
                if (max < tmp)
                    max = tmp;
            }
            return max + 1;
        }

        public void Print()
        {
            Console.WriteLine();
            Console.WriteLine("CoordsX CoordsY");
            for (int i = 0; i < CoordsY.Length; i++)
            {
                Console.WriteLine(" id {0} x {1:F4} y {2:F4}", i, CoordsX[i], CoordsY[i]);
            }
            Console.WriteLine();
            Console.WriteLine("BoundKnots");
            for (int i = 0; i < BoundKnots.Length; i++)
            {
                Console.WriteLine(" id {0} ", BoundKnots[i]);
            }
            Console.WriteLine();
            Console.WriteLine("FE");
            for (int i = 0; i < AreaElems.Length; i++)
            {
                int ID = i;
                uint n0 = AreaElems[i].Vertex1;
                uint n1 = AreaElems[i].Vertex2;
                uint n2 = AreaElems[i].Vertex3;
                Console.WriteLine(" id {0}: {1} {2} {3}", ID, n0, n1, n2);
            }
            Console.WriteLine();
            Console.WriteLine("BFE");
            for (int i = 0; i < BoundElems.Length; i++)
            {
                uint n0 = BoundElems[i].Vertex1;
                uint n1 = BoundElems[i].Vertex2;
                int fl = BoundElementsFlag[i];
                Console.WriteLine(" id {0}: {1} {2} fl {3}", i, n0, n1, fl);
            }
        }
    }
}