//---------------------------------------------------------------------------
//                        Потапов И.И.
//                 - (F) CopyFree 2002 -
//                  Last Edit Data: 25.06.2001
//---------------------------------------------------------------------------
//            "Класс строк для матриц с полной упаковкой "
//---------------------------------------------------------------------------
//                  перенос с C++ ==> C#
//                       18.04.2021    
//---------------------------------------------------------------------------
namespace BedLoadTask.Algebra
{
    using System;
    /// <summary>
    /// ОО: Используется для хранения разреженных матриц формате CRS
    /// </summary>
    public class SparseElement : IComparable<SparseElement>
    {
        /// <summary>
        /// значение ненулевого элемента в строке
        /// </summary>
        public double Elem;
        /// <summary>
        /// индекс (j) ненулевого элемента в строке
        /// </summary>
        public int Knot;
        public SparseElement() { }
        public SparseElement(double _Elem, int _Knot)
        {
            Elem = _Elem; Knot = _Knot;
        }
        public static bool operator <=(SparseElement a, SparseElement b)
        {
            return a.Knot <= b.Knot;
        }
        public static bool operator >=(SparseElement a, SparseElement b)
        {
            return a.Knot >= b.Knot;
        }
        public int CompareTo(SparseElement b)
        {
            if (Knot > b.Knot)
                return 1;
            else if (Knot < b.Knot)
                return -1;
            else
                return 0;
        }
        //public static bool operator ==(SparseElement a, SparseElement b )
        //{ 
        //    return a.Knot == b.Knot; 
        //}
        //public static bool operator !=(SparseElement a, SparseElement b)
        //{ 
        //    return a.Knot != b.Knot; 
        //}
    }
}
