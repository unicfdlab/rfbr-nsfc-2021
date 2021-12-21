//---------------------------------------------------------------------------
//                 Реализация библиотеки для моделирования 
//                  гидродинамических и русловых процессов
//---------------------------------------------------------------------------
//                    - (C) Copyright 2020
//                      ALL RIGHT RESERVED
//                       проектировщик:
//                         Потапов И.И.
//                           12.07.21
//---------------------------------------------------------------------------
namespace BedLoadTask.Log
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    /// <summary>
    /// ОО: Класс логгер
    /// </summary>
    public static class LOG
    {
        public static int flagLogger = 0;
        public static string FileLogger = "FileLogger.txt";
        public static NumberFormatInfo formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };

        /// <summary>
        /// добавление строки в файл
        /// </summary>
        /// <param name="message"></param>
        public static void sendToFile(string message)
        {
            using (StreamWriter writer = new StreamWriter(FileLogger, true))
            {
                writer.WriteLine(message);
                writer.Close();
            }
        }
        public static void LogMessage(string mes)
        {
            if (flagLogger == 0)
                Console.WriteLine(" INFO: " + mes);
            else
                sendToFile(" INFO: " + mes);
        }
        public static void LogErr(string mes)
        {
            if (flagLogger == 0)
                Console.WriteLine("ERROR: " + mes);
            else
                sendToFile(" INFO: " + mes);
        }

        public static void Print<Type>(string Name, Type value) where Type : struct
        {
            string s = String.Format(Name + " = " + value.ToString());
            if (flagLogger == 0)
                Console.WriteLine(s);
            else
                sendToFile(s);
        }
        public static void Print(string Name, string value)
        {
            string s = String.Format(Name + " = " + value);
            if (flagLogger == 0)
                Console.WriteLine(s);
            else
                sendToFile(s);
        }
        /// <summary>
        /// Тестовая печать поля
        /// </summary>
        /// <param name="Name">имя поля</param>
        /// <param name="mas">массив пля</param>
        /// <param name="FP">точность печати</param>
        public static void PrintMas(string Name, double[] mas, int FP = 8)
        {
            string Format = " {0:F6}";
            if (FP != 6)
                Format = " {0:F" + FP.ToString() + "}";
            if (flagLogger == 0)
            {
                Console.WriteLine(Name);
                for (int i = 0; i < mas.Length; i++)
                {
                    Console.Write(Format, mas[i]);
                }
                Console.WriteLine();
            }
            else
            {
                using (StreamWriter writer = new StreamWriter(FileLogger, true))
                {
                    writer.WriteLine(Name);
                    for (int i = 0; i < mas.Length; i++)
                    {
                        writer.Write(Format, mas[i]);
                    }
                    writer.WriteLine();
                    writer.Close();
                }
            }
        }
        /// <summary>
        /// Печать ЛМЖ для отладки
        /// </summary>
        /// <param name="M"></param>
        /// <param name="Count"></param>
        /// <param name="F"></param>
        public static void Print(string Name, double[][] M, int F = 2)
        {
            string SF = "F" + F.ToString();
            if (flagLogger == 0)
            {
                Console.WriteLine(Name);
                for (int i = 0; i < M.Length; i++)
                {
                    Console.Write("{0} ", i);
                    for (int j = 0; j < M[i].Length; j++)
                        Console.Write(" " + M[i][j].ToString(SF));
                    Console.WriteLine();
                }
            }
            else
            {
                using (StreamWriter writer = new StreamWriter(FileLogger, true))
                {
                    writer.WriteLine(Name);
                    for (int i = 0; i < M.Length; i++)
                    {
                        writer.Write("{0} ", i);
                        for (int j = 0; j < M[i].Length; j++)
                            writer.Write(" " + M[i][j].ToString(SF));
                        writer.WriteLine();
                    }
                    writer.Close();
                }
            }
        }
        public static void Print<T>(string Name, T[][] M) where T : struct
        {
            Console.WriteLine(Name);
            for (int i = 0; i < M.Length; i++)
            {
                for (int j = 0; j < M[i].Length; j++)
                {
                    Console.Write(" ");
                    Console.Write(M[i][j]);
                }
                Console.WriteLine();
            }
        }
        /// <summary>
        /// Печать ЛМЖ для отладки
        /// </summary>
        /// <param name="M"></param>
        /// <param name="Count"></param>
        /// <param name="F"></param>
        public static void Print(string Name, double[,] M, int F = 2)
        {
            Console.WriteLine(Name);
            string SF = "F" + F.ToString();
            int NX = M.GetLength(0);
            for (int i = 0; i < NX; i++)
            {
                int NY = M.GetLength(1);
                for (int j = 0; j < NY; j++)
                    Console.Write(" " + M[i, j].ToString(SF));
                Console.WriteLine();
            }
        }
        public static void Print<T>(string Name, T[,] M) where T : struct
        {
            Console.WriteLine(Name);
            for (int i = 0; i < M.GetLength(0); i++)
            {
                for (int j = 0; j < M.GetLength(1); j++)
                {
                    Console.Write(" ");
                    Console.Write(M[i, j]);
                }
                Console.WriteLine();
            }
        }

        public static void Print(string Name, double[] M, int F = 2)
        {
            Console.WriteLine(Name);
            string SF = "F" + F.ToString();
            for (int i = 0; i < M.Length; i++)
                Console.Write(" " + M[i].ToString(SF));
            Console.WriteLine();
        }
        public static void Print(string Name, double[] M, int N, int F = 2)
        {
            Console.WriteLine(Name);
            string SF = "F" + F.ToString();
            for (int i = 0; i < M.Length; i++)
            {
                if (i % N == 0)
                    Console.WriteLine();
                Console.Write(" " + M[i].ToString(SF));
            }
            Console.WriteLine();
        }
        public static void Print(string Name, int[] M)
        {
            Console.WriteLine(Name);
            for (int i = 0; i < M.Length; i++)
                Console.Write(" " + M[i].ToString());
            Console.WriteLine();
        }
        public static void Print(string Name, uint[] M)
        {
            Console.WriteLine(Name);
            for (int i = 0; i < M.Length; i++)
                Console.Write(" " + M[i].ToString());
            Console.WriteLine();
        }


        /// <summary>
        /// Загрузка сетки КЭ, координат вершин и дна, придонных касательных напряжений и давления
        /// </summary>
        /// <param name="FileName">имя файла</param>
        /// <param name="V">напряжение ...</param>
        public static void LoadMasLine(string FileName, ref double[] V, int N)
        {
            using (StreamReader file = new StreamReader(FileName))
            {
                if (file == null)
                {
                    LOG.LogMessage("штатно не открыт файл " + FileName);
                }
                string line = file.ReadLine().Trim('\t');
                string[] slines = line.Split(' ', '\t');
                LOG.LogMessage(line);
                V = new double[N];
                List<double> list = new List<double>();
                for (int i = 0; i < slines.Length; i++)
                {
                    string s = slines[i];
                    if (s.Length > 0)
                    {
                        list.Add(double.Parse(s, formatter));
                        Console.Write(s + " ");
                    }
                }
                V = list.ToArray();
                LOG.LogMessage("Загружен файл " + FileName);


                // var numbers = Console.ReadLine().Split(' ')
                //.Where(x => { int i; return int.TryParse(x, out i); })
                //.Select(int.Parse).ToArray();
            }
        }
        /// <summary>
        /// Запись в файл одномерного массива
        /// </summary>
        /// <param name="FileName">имя файла</param>
        /// <param name="V">массив</param>
        /// <param name="FP">мантиса числе</param>
        public static void SaveMasLine(string FileName, double[] V, int FP = 8)
        {
            string Format = " {0:F" + FP.ToString() + "}";
            using (StreamWriter file = new StreamWriter(FileName))
            {
                for (int i = 0; i < V.Length; i++)
                    file.Write(Format, V[i]);
                file.Close();
            }
        }
    }
}
