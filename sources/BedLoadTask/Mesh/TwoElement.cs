//---------------------------------------------------------------------------
//                          ПРОЕКТ  "МКЭ"
//                         проектировщик:
//                           Потапов И.И.
//---------------------------------------------------------------------------
//                         ПРОЕКТ  "River"
//                 правка  :   06.12.2020 Потапов И.И.
//---------------------------------------------------------------------------
namespace BedLoadTask.Mesh
{
    using System;
    using System.Runtime.InteropServices;
    [StructLayout(LayoutKind.Sequential)] // фиксация порядка полей при экспорте в с++
    [Serializable]
    public struct TwoElement
    {
        /// <summary>
        /// Первая точка треугольника
        /// </summary>
        public uint Vertex1;
        /// <summary>
        /// Вторая точка треугольника
        /// </summary>
        public uint Vertex2;

        public TwoElement(uint upLeft, uint lowRight)
        {
            this.Vertex1 = upLeft;
            this.Vertex2 = lowRight;
        }
    }
}
