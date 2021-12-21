//---------------------------------------------------------------------------
//                          ПРОЕКТ  "МКЭ"
//                         проектировщик:
//                           Потапов И.И.
//---------------------------------------------------------------------------
//                         ПРОЕКТ  "RiverLib"
//                 правка  :   06.12.2020 Потапов И.И.
//---------------------------------------------------------------------------
namespace BedLoadTask.Mesh
{
    using System;
    using System.Runtime.InteropServices;
    /// <summary>
    /// ОО: Структура TriElement определяет индекс трех
    /// вершины в массиве pVertex в функции GradientFill.
    /// Эти три вершины образуют один треугольник
    /// </summary>
    [StructLayout(LayoutKind.Sequential)] // фиксация порядка полей при экспорте в с++
    [Serializable]
    public struct TriElement
    {
        /// <summary>
        /// Первая точка треугольника
        /// </summary>
        public uint Vertex1;
        /// <summary>
        /// Вторая точка треугольника
        /// </summary>
        public uint Vertex2;
        /// <summary>
        /// Третья точка треугольника
        /// </summary>
        public uint Vertex3;
        public TriElement(uint vertex1, uint vertex2, uint vertex3)
        {
            this.Vertex1 = vertex1;
            this.Vertex2 = vertex2;
            this.Vertex3 = vertex3;
        }

    }
}