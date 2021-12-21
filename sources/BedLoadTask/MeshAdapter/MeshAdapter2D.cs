//---------------------------------------------------------------------------
//                    ПРОЕКТ  "РУСЛОВЫЕ ПРОЦЕССЫ"
//                         проектировщик:
//                           Потапов И.И.
//---------------------------------------------------------------------------
//                 кодировка : 14.12.2020 Потапов И.И.
//---------------------------------------------------------------------------
//                 изменения : 18.11.2021 Потапов И.И.
//                   для выравнивания строк в файлах
//             добавлены методы ConvertToString(...), Split(...)
//---------------------------------------------------------------------------
namespace BedLoadTask.MeshAdapter
{
    using BedLoadTask.Log;
    using BedLoadTask.Mesh;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    /// <summary>
    /// ОО: Загрузка сетки и данных
    /// </summary>
    public class MeshAdapter2D
    {
        public static NumberFormatInfo formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
        /// <summary>
        /// Загрузка сетки КЭ, координат вершин и дна, придонных касательных напряжений и давления
        /// </summary>
        /// <param name="FileName">имя файла</param>
        /// <param name="mesh">сетка</param>
        /// <param name="Zeta0">дно</param>
        /// <param name="tauX">придонное касательное напряжение по Х</param>
        /// <param name="tauY">придонное касательное напряжение по Х</param>
        /// <param name="P">придонное давление</param>
        public void LoadData(string FileName, ref TriMesh mesh, ref double[] Zeta0, ref double[] tauX, ref double[] tauY, ref double[] P)
        {
            mesh = new TriMesh();
            using (StreamReader file = new StreamReader(FileName))
            {
                if (file == null)
                {
                    LOG.LogMessage("штатно не открыт файл " + FileName);
                }
                string line = file.ReadLine();
                int Count = int.Parse(line.Trim('\t'));
                LOG.LogMessage(line);
                mesh.AreaElems = new TriElement[Count];
                for (int i = 0; i < Count; i++)
                {
                    line = file.ReadLine().Trim();
                    string[] slines = line.Split(',', '(', ')', ' ', '\t');
                    mesh.AreaElems[i].Vertex1 = uint.Parse(slines[0]);
                    mesh.AreaElems[i].Vertex2 = uint.Parse(slines[1]);
                    mesh.AreaElems[i].Vertex3 = uint.Parse(slines[2]);
                    // LOG.LogMessage(" fe = "+i.ToString() +" "+line);
                }
                line = file.ReadLine();
                LOG.LogMessage("CountCoord = " + line);
                int CountCoord = int.Parse(line.Trim('\t'));
                Zeta0 = new double[CountCoord];
                mesh.CoordsX = new double[CountCoord];
                mesh.CoordsY = new double[CountCoord];
                for (int i = 0; i < CountCoord; i++)
                {
                    line = file.ReadLine().Trim('(', ')', '\t');
                    //string[] slines1 = line.Split(' ', '\t');
                    string[] slines = Split(line);
                    mesh.CoordsX[i] = double.Parse(slines[0], formatter);
                    mesh.CoordsY[i] = double.Parse(slines[1], formatter);
                    Zeta0[i] = double.Parse(slines[2], formatter);
                    //   LOG.LogMessage(" knot = " + i.ToString() + " " + line);
                }
                // исключение плохих КЭ
                uint[] kn = { 0, 0, 0 };
                double[] xx = { 0, 0, 0 };
                double[] yy = { 0, 0, 0 };
                List<TriElement> goodElems = new List<TriElement>();
                int CoundFL = 0;
                double S;
                int flag;
                using (StreamWriter fileRep = new StreamWriter("ZeroElements.txt"))
                {
                    for (uint elem = 0; elem < mesh.CountElement; elem++)
                    {
                        flag = 0;
                        // получить узлы КЭ
                        mesh.ElementKnots(elem, ref kn);
                        // координаты и площадь
                        mesh.ElemX(elem, ref xx);
                        mesh.ElemY(elem, ref yy);
                        if (xx[0] == xx[1] && xx[1] == xx[2])
                            flag = 1;
                        if (yy[0] == yy[1] && yy[1] == yy[2])
                            flag += 2;
                        // площадь
                        S = mesh.ElemSquare(xx, yy);
                        if (S != 0 && flag == 0)
                        {
                            TriElement e = mesh.AreaElems[elem];
                            goodElems.Add(e);
                        }
                        else
                        {
                            fileRep.WriteLine("Площадь FE = {0:F10} Index FE {1}", S, elem);
                            //    Console.WriteLine("S = {0:F10} Index FE {1} flag = {2} ", S, elem, flag);
                            CoundFL++;
                        }
                    }
                    fileRep.WriteLine("Плохих элементов {0}", CoundFL);
                    LOG.LogMessage("Плохих элементов " + CoundFL.ToString());
                    fileRep.Close();
                }
                mesh.AreaElems = goodElems.ToArray();


                tauX = new double[CountCoord];
                for (int i = 0; i < CountCoord; i++)
                {
                    line = file.ReadLine().Trim('(', ')', '\t');
                    //string[] slines = line.Split(' ', '\t');
                    string[] slines = Split(line);
                    tauX[i] = double.Parse(slines[0], formatter);
                    //   LOG.LogMessage(" tauX = " + i.ToString() + " " + line);
                }
                tauY = new double[CountCoord];
                for (int i = 0; i < CountCoord; i++)
                {
                    line = file.ReadLine().Trim('(', ')', '\t');
                    //string[] slines = line.Split(' ', '\t');
                    string[] slines = Split(line);
                    tauY[i] = double.Parse(slines[0], formatter);
                    //   LOG.LogMessage(" tauY = " + i.ToString() + " " + line);
                }
                P = new double[CountCoord];
                for (int i = 0; i < CountCoord; i++)
                {
                    line = file.ReadLine().Trim('(', ')', '\t');
                    //string[] slines = line.Split(' ', '\t');
                    string[] slines = Split(line);
                    P[i] = double.Parse(slines[0], formatter);
                    //  LOG.LogMessage(" P = " + i.ToString() + " " + line);
                }
                double[] x = mesh.GetCoordsX();
                double[] y = mesh.GetCoordsY();
                double maxX = x.Max();
                double minX = x.Min();
                double maxY = y.Max();
                double minY = y.Min();
                List<int> knotB = new List<int>();
                List<int> knotR = new List<int>();
                List<int> knotT = new List<int>();
                List<int> knotL = new List<int>();

                List<int> FB = new List<int>();
                List<int> FR = new List<int>();
                List<int> FT = new List<int>();
                List<int> FL = new List<int>();

                for (int i = 0; i < y.Length; i++)
                    if (Math.Abs(y[i] - minY) < 0.00001)
                    { knotB.Add(i); FB.Add(0); }
                knotB.Sort();
                for (int i = 0; i < y.Length; i++)
                    if (Math.Abs(x[i] - maxX) < 0.00001)
                    { knotR.Add(i); FR.Add(1); }
                knotR.Sort();
                for (int i = 0; i < y.Length; i++)
                    if (Math.Abs(y[i] - maxY) < 0.00001)
                    { knotT.Add(i); FT.Add(2); }
                knotT.Sort();
                for (int i = 0; i < y.Length; i++)
                    if (Math.Abs(x[i] - minX) < 0.00001)
                    { knotL.Add(i); FL.Add(3); }
                knotL.Sort();
                List<int> knots = new List<int>();
                List<int> FLG = new List<int>();
                knots.AddRange(knotB);
                knots.AddRange(knotR);
                knots.AddRange(knotT);
                knots.AddRange(knotL);

                FLG.AddRange(FB);
                FLG.AddRange(FR);
                FLG.AddRange(FT);
                FLG.AddRange(FL);

                mesh.BoundKnots = knots.ToArray();
                mesh.BoundKnotsFlag = FLG.ToArray();

                uint CBE = (uint)mesh.BoundKnots.Length;
                mesh.BoundElems = new TwoElement[CBE];
                mesh.BoundElementsFlag = new int[CBE];
            }
        }
        /// <summary>
        /// Сохранение сетки КЭ, координат вершин и дна,
        /// </summary>
        /// <param name="FileName">имя файла</param>
        /// <param name="mesh">сетка</param>
        /// <param name="Zeta0">дно</param>
        /// <param name="tauX">придонное касательное напряжение по Х</param>
        /// <param name="tauY">придонное касательное напряжение по Х</param>
        /// <param name="P">придонное давление</param>
        public void SaveData(string FileName, TriMesh mesh, double[] Zeta0, double[] tauX, double[] tauY, double[] P)
        {
            using (StreamWriter file = new StreamWriter(FileName))
            {
                file.WriteLine(mesh.CountElement);
                for (int i = 0; i < mesh.CountElement; i++)
                {
                    file.WriteLine(mesh.AreaElems[i].Vertex1.ToString() + " " +
                                   mesh.AreaElems[i].Vertex2.ToString() + " " +
                                   mesh.AreaElems[i].Vertex3.ToString());
                }
                file.WriteLine(mesh.Count);
                for (int i = 0; i < mesh.Count; i++)
                {
                    string x = ConvertToString(mesh.CoordsX[i], "F8");
                    string y = ConvertToString(mesh.CoordsY[i], "F8");
                    string z = ConvertToString(Zeta0[i], "F8");
                    file.WriteLine(x + " " + y + " " + z);
                    //file.WriteLine("{0:F8} {1:F8} {2:F8}",
                    //mesh.CoordsX[i],
                    //mesh.CoordsY[i],
                    //Zeta0[i]);
                }
                for (int i = 0; i < mesh.Count; i++)
                    file.WriteLine(ConvertToString(tauX[i], "F8"));
                for (int i = 0; i < mesh.Count; i++)
                    file.WriteLine(ConvertToString(tauY[i], "F8"));
                for (int i = 0; i < mesh.Count; i++)
                    file.WriteLine(ConvertToString(P[i], "F8"));
                file.Close();
            }
        }

        public string ConvertToString(double a,string F)
        {
            string s = a.ToString(F);
            if (a > 0)
                s = " " + s;
            return s;
        }

        public string[] Split(string line)
        {
            string[] slines = line.Split(' ', '\t');
            int k = 0;
            for(int i=0; i<slines.Length; i++)
                if(slines[i]!="")
                    slines[k++] = slines[i];
            return slines;
        }
        

        /// <summary>
        /// Сохранение координат вершин
        /// </summary>
        /// <param name="FileName">имя файла</param>
        /// <param name="mesh">сетка</param>
        /// <param name="Zeta0">дно</param>
        public void SaveDataZeta(string FileName, TriMesh mesh, double[] Zeta0)
        {
            using (StreamWriter file = new StreamWriter(FileName))
            {
                file.WriteLine(mesh.Count);
                for (int i = 0; i < mesh.Count; i++)
                {
                    //string x = ConvertToString(mesh.CoordsX[i], "F8");
                    //string y = ConvertToString(mesh.CoordsY[i], "F8");
                    //string z = ConvertToString(Zeta0[i], "F8");
                    //file.WriteLine(x + " " + y + " " + z);
                    file.WriteLine("{0:F8} {1:F8} {2:F8}",
                    mesh.CoordsX[i],
                    mesh.CoordsY[i],
                    Zeta0[i]);
                }
                file.Close();
            }
        }

        /// <summary>
        /// Печатаем сетку на консоль
        /// </summary>
        /// <param name="mesh">Экземпляр интерфейса<see cref="IMesh" /> interface.</param>
        public void WriteTriMesh(TriMesh mesh)
        {
            if (mesh == null)
                throw new NotSupportedException("Не показать выбранный формат сетки mesh == null");

            for (int i = 0; i < mesh.CountElement; i++)
            {
                Console.WriteLine(mesh.AreaElems[i].Vertex1.ToString() + " " +
                               mesh.AreaElems[i].Vertex2.ToString() + " " +
                               mesh.AreaElems[i].Vertex3.ToString());
            }
            Console.WriteLine(mesh.Count);
            for (int i = 0; i < mesh.Count; i++)
            {
                Console.WriteLine("{0} {1}",
                mesh.CoordsX[i],
                mesh.CoordsY[i]);
            }
            Console.WriteLine(mesh.CountBoundElement);
            for (int i = 0; i < mesh.CountBoundElement; i++)
            {
                Console.WriteLine(mesh.BoundElems[i].Vertex1.ToString() + " " +
                               mesh.BoundElems[i].Vertex2.ToString() + " " +
                               mesh.BoundElementsFlag[i].ToString());
            }
            Console.WriteLine(mesh.BoundKnots.Length);
            for (int i = 0; i < mesh.BoundKnots.Length; i++)
            {
                Console.WriteLine("{0} {1}",
                mesh.BoundKnots[i],
                mesh.BoundKnotsFlag[i]);
            }
        }

        // ======================================================
        //       Система координат        Обход узлов
        //     dy                                     i
        //   |---|----------> Y j      -------------> 0 j
        //   | dx                      -------------> 1 j 
        //   |---                      -------------> 2 j
        //   |
        //   |
        //   |
        //   V X  i
        // ======================================================
        /// <summary>
        /// Генерация триангуляционной КЭ сетки в прямоугольной области
        /// </summary>
        /// <param name="mesh">результат</param>
        /// <param name="Nx">узлов по Х</param>
        /// <param name="Ny">узлов по У</param>
        /// <param name="dx">шаг по Х</param>
        /// <param name="dy">шаг по У</param>
        /// <param name="Flag">признаки границ для ГУ</param>
        public void GetRectsngleMesh(ref TriMesh mesh, int Nx, int Ny, double dx, double dy, int[] Flag = null)
        {
            int[] LFlag = { 0, 1, 1, 0 };
            if (Flag == null)
                Flag = LFlag;

            mesh = new TriMesh();
            int counter = 2 * (Nx - 1) + 2 * (Ny - 1);
            int CountNodes = Nx * Ny;
            int CountElems = 2 * (Nx - 1) * (Ny - 1);

            mesh.AreaElems = new TriElement[CountElems];

            mesh.BoundElems = new TwoElement[counter];
            mesh.BoundElementsFlag = new int[counter];

            mesh.BoundKnots = new int[counter];
            mesh.BoundKnotsFlag = new int[counter];

            mesh.CoordsX = new double[CountNodes];
            mesh.CoordsY = new double[CountNodes];

            uint[,] map = new uint[Nx, Ny];

            uint k = 0;
            for (uint i = 0; i < Nx; i++)
            {
                double xm = i * dx;
                for (int j = 0; j < Ny; j++)
                {
                    double ym = dy * j;
                    mesh.CoordsX[k] = xm;
                    mesh.CoordsY[k] = ym;
                    map[i, j] = k++;
                }
            }
            //for (int j = 0; j < Ny; j++)
            //{
            //    for (uint i = 0; i < Nx; i++)
            //    {
            //        double ym = j * dx;
            //        double xm = i * dy;

            //        mesh.CoordsX[k] = xm;
            //        mesh.CoordsY[k] = ym;
            //        map[i, j] = k++;
            //    }
            //}




            int elem = 0;
            for (int i = 0; i < Nx - 1; i++)
            {
                for (int j = 0; j < Ny - 1; j++)
                {
                    mesh.AreaElems[elem] = new TriElement(map[i, j], map[i + 1, j], map[i + 1, j + 1]);
                    elem++;
                    mesh.AreaElems[elem] = new TriElement(map[i + 1, j + 1], map[i, j + 1], map[i, j]);
                    elem++;
                }
            }
            k = 0;


            // низ
            for (int i = 0; i < Ny - 1; i++)
            {
                mesh.BoundElems[k] = new TwoElement(map[Nx - 1, i], map[Nx - 1, i + 1]);
                mesh.BoundElementsFlag[k] = Flag[0];
                // задана функция
                mesh.BoundKnotsFlag[k] = Flag[0];
                mesh.BoundKnots[k++] = (int)map[Nx - 1, i];
            }
            // правая сторона
            for (int i = 0; i < Nx - 1; i++)
            {
                mesh.BoundElems[k] = new TwoElement(map[Nx - 1 - i, Ny - 1], map[Nx - 2 - i, Ny - 1]);
                mesh.BoundElementsFlag[k] = Flag[1];
                // задана производная
                mesh.BoundKnotsFlag[k] = Flag[1];
                mesh.BoundKnots[k++] = (int)map[Nx - 1 - i, Ny - 1];
            }
            // верх
            for (int i = 0; i < Ny - 1; i++)
            {
                mesh.BoundElems[k] = new TwoElement(map[0, Ny - i - 1], map[0, Ny - i - 2]);
                mesh.BoundElementsFlag[k] = Flag[2];
                // задана производная
                mesh.BoundKnotsFlag[k] = Flag[2];
                mesh.BoundKnots[k++] = (int)map[0, Ny - i - 1];
            }
            // левая сторона
            for (int i = 0; i < Nx - 1; i++)
            {
                mesh.BoundElems[k] = new TwoElement(map[i, 0], map[i + 1, 0]);
                mesh.BoundElementsFlag[k] = Flag[3];
                // задана функция
                mesh.BoundKnotsFlag[k] = Flag[3];
                mesh.BoundKnots[k++] = (int)map[i, 0];
            }

        }
    }
}
