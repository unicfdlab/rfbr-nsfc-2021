//---------------------------------------------------------------------------
//                          ПРОЕКТ  "H?V"
//                         проектировщик:
//                           Потапов И.И.
//---------------------------------------------------------------------------
//             кодировка : 2.10.2000 Потапов И.И. (c++)
//---------------------------------------------------------------------------
//                          ПРОЕКТ  "МКЭ"
//                         проектировщик:
//                           Потапов И.И.
//---------------------------------------------------------------------------
//           кодировка : 25.12.2020 Потапов И.И. (c++=> c#)
//---------------------------------------------------------------------------
namespace BedLoadTask.Mesh
{
    using System;
    /// <summary>
    /// Определение класса HPoint Точка в (2) мерной системе координат
    /// </summary>
    [Serializable]
    public class HPoint
    {
        public double x;
        public double y;
        public HPoint()
        {
            x = 0;
            y = 0;
        }
        public HPoint(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        public HPoint(HPoint a)
        {
            x = a.x;
            y = a.y;
        }
        public static double Length(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
        }
        public static double Length(HPoint a, HPoint b)
        {
            return Math.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y));
        }
        // расстояние мж точкой текущей точкой и точкой b
        private double LengthAB(HPoint b)
        {
            return Math.Sqrt((x - b.x) * (x - b.x) + (y - b.y) * (y - b.y));
        }
        // длина радиус вектора для точки а
        public double Scalar(HPoint a)
        {
            return Math.Sqrt(x * a.x + y * a.y);
        }
        // норма точки а
        public double Norm()
        {
            return Math.Sqrt(x * x + y * y);
        }
        public static HPoint operator +(HPoint pb, HPoint p2)
        {
            return new HPoint(pb.x + p2.x, pb.y + p2.y);
        }
        public static HPoint operator -(HPoint pb, HPoint p2)
        {
            return new HPoint(pb.x - p2.x, pb.y - p2.y);
        }

        //public static HPoint operator *(double scal, HPoint p2)
        //{
        //    return new HPoint(scal * p2.x, scal * p2.y);
        //}
    }
}
