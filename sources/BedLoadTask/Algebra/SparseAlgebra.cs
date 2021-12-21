//---------------------------------------------------------------------------
//                        Потапов И.И.
//                  - (F) CopyFree 2001 -
//              member-функции класса HPackAlgebra
//                  Last Edit Data: 25.6.2001
//---------------------------------------------------------------------------
//                        Потапов И.И.
//          HPackAlgebra (C++) ==> SparseAlgebra (C#)
//                       18.04.2021 
//---------------------------------------------------------------------------
namespace BedLoadTask.Algebra
{
    using BedLoadTask.Log;
    using System;
    using System.Collections.Generic;
    /// <summary>
    /// ОО: Класс матрицы в формате CRS
    /// с поддержкой интерфейса алгебры для работы с КЭ задачами
    /// </summary>
    public abstract class SparseAlgebra : IAlgebra
    {
        /// <summary>
        /// Флаг отладки
        /// </summary>
        public bool Debug = false;
        /// <summary>
        /// Точность
        /// </summary>
        public static double EPS = MEM.Error8;
        /// <summary>
        /// Название метода
        /// </summary>
        public string Name { get => name; }
        /// <summary>
        /// порядок СЛУ
        /// </summary>
        public uint N { get => FN; }
        /// <summary>
        /// порядок СЛУ
        /// </summary>
        protected uint FN;
        /// <summary>
        /// плотно упакованная матрица системы
        /// </summary>
        public List<SparseRow> Matrix;
        /// <summary>
        /// вектор правой части
        /// </summary>
        public double[] Right;
        /// <summary>
        /// имя метода
        /// </summary>
        protected string name;
        public SparseAlgebra(uint FN = 1)
        {
            SetSparseAlgebra(FN);
        }
        public virtual void SetSparseAlgebra(uint FN)
        {
            this.FN = FN;
            Matrix = new List<SparseRow>();
            Right = new double[FN];
            for (int i = 0; i < FN; i++)
                Matrix.Add(new SparseRow());
        }
        /// <summary>
        /// Очистка матрицы и правой части
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < N; i++)
            {
                Matrix[i].Row.Clear();
                Right[i] = 0;
            }
        }
        /// <summary>
        /// Сборка ГМЖ
        /// </summary>
        public void AddToMatrix(double[][] LMartix, uint[] Adress)
        {
            for (int a = 0; a < Adress.Length; a++)
            {
                int ash = (int)Adress[a];
                Matrix[ash].Add(LMartix[a], Adress);
            }
        }
        //---------------------------------------------------------------------------
        public void AddToRight(double[] LRight, uint[] Adress)
        {
            // цикл по узлам КЭ
            for (int a = 0; a < Adress.Length; a++)
                Right[Adress[a]] += LRight[a];
        }
        /// <summary>
        /// Сборка САУ по строкам (не для всех решателей)
        /// </summary>
        /// <param name="ColElems">Коэффициенты строки системы</param>
        /// <param name="ColAdress">Адреса коэффицентов</param>
        /// <param name="IndexRow">Индекс формируемой строки системы</param>
        /// <param name="Right">Значение правой части строки</param>
        public void AddStringSystem(double[] ColElems, uint[] ColAdress, uint IndexRow, double R)
        {
            Matrix[(int)IndexRow].Add(ColElems, ColAdress);
            Right[IndexRow] += R;
        }
        /// <summary>
        /// Добавление в правую часть
        /// </summary>
        public void CopyRight(double[] CRight)
        {
            for (int a = 0; a < Right.Length; a++)
                Right[a] += CRight[a];
        }
        /// <summary>
        /// Удовлетворение ГУ
        /// </summary>
        public void BoundConditions(double[] Conditions, uint[] Adress)
        {
            for (int i = 0; i < Adress.Length; i++)
            {
                int a = (int)Adress[i];
                Matrix[a].Row.Clear();
                Matrix[a].Add(new SparseElement(1.0, a));
                Right[a] = Conditions[i];
            }
        }
        /// <summary>
        /// Выполнение ГУ
        /// </summary>
        public void BoundConditions(double Condition, uint[] Adress)
        {
            for (int i = 0; i < Adress.Length; i++)
            {
                int a = (int)Adress[i];
                Matrix[a].Row.Clear();
                Matrix[a].Row.Add(new SparseElement(1.0, a));
                Right[a] = Condition;
            }
        }
        /// <summary>
        /// Операция умножения вектора на матрицу
        /// </summary>
        /// <param name="R">результат</param>
        /// <param name="X">умножаемый вектор</param>
        /// <param name="IsRight">знак операции = +/- 1</param>
        public void getResidual(ref double[] R, double[] X, int IsRight = 1)
        {
            if (R == null)
                R = new double[X.Length];
            //for (int i = 0; i < X.Length; i++)
            //    R[i] = Matrix[i] * X;
            for (int i = 0; i < Matrix.Count; i++)
            {
                double sum = 0;
                for (int j = 0; j < Matrix[i].Row.Count; j++)
                {
                    int k = Matrix[i].Row[j].Knot;
                    sum += Matrix[i].Row[j].Elem * X[k];
                }
                R[i] = sum;
            }
        }
        /// <summary>
        /// Скалярное произведение векторов
        /// </summary>
        protected double MultyVector(double[] a, double[] b)
        {
            double Sum = 0;
            for (uint i = 0; i < N; i++)
                Sum += a[i] * b[i];
            return Sum;
        }

        public void CheckMas(double[] MM, string mes = "")
        {
            if (Debug != true) return;
            Console.WriteLine("щаг отладки " + mes);
            for (int i = 0; i < MM.Length; i++)
                if (double.IsNaN(MM[i]) == true || double.IsInfinity(MM[i]) == true)
                    Console.WriteLine(" i =" + i.ToString());
        }
        public int CheckSYS(string mes = "")
        {
            if (Debug != true) return -1;
            Console.WriteLine("щаг отладки " + mes);
            int ai = -1; int aj = -1;
            for (int i = 0; i < N; i++)
            {
                if (double.IsNaN(Right[i]) == true || double.IsInfinity(Right[i]) == true)
                {
                    Console.WriteLine(" i =" + i.ToString());
                }
            }
            for (ai = 0; ai < N; ai++)
            {
                for (aj = 0; aj < Matrix[ai].Count; aj++)
                {
                    if (double.IsNaN(Matrix[ai].Row[aj].Elem) == true
                        || double.IsInfinity(Matrix[ai].Row[aj].Elem))
                        Console.WriteLine(" i = {0}, j = {1}", ai, aj);
                }
            }
            Console.WriteLine("матрица проверена");
            ai = -1; aj = -1;
            return -1;
        }
        /// <summary>
        /// Решение СЛУ
        /// </summary>
        public abstract void Solve(ref double[] X);
        /// <summary>
        /// Клонирование объекта
        /// </summary>
        /// <returns></returns>
        public abstract IAlgebra Clone();
        /// <summary>
        /// Вывод САУ на КОНСОЛЬ
        /// </summary>
        public virtual void Print()
        {
            Console.WriteLine("Matrix");
            for (int i = 0; i < N; i++)
            {
                double[] buf = new double[N];
                SparseRow.DeCompress(Matrix[i], ref buf);
                Console.Write("{0} ", i);
                for (int j = 0; j < N; j++)
                    Console.Write(" " + buf[j].ToString("F4"));
                Console.WriteLine();
            }
            Console.WriteLine("Right");
            for (int i = 0; i < N; i++)
                Console.Write(" " + Right[i].ToString("F4"));
            Console.WriteLine();
        }
    }
}
