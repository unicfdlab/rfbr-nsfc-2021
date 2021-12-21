//---------------------------------------------------------------------------
//                      Усеченная библиотека контроля
//                              Потапов И.И.
//                               12.04.20
//---------------------------------------------------------------------------
namespace BedLoadTask.Log
{
    public static class MEM
    {
        public static double Error3 => 0.001;
        public static double Error4 => 0.0001;
        public static double Error5 => 0.00001;
        public static double Error6 => 0.000001;
        public static double Error7 => 0.0000001;
        public static double Error8 => 0.00000001;
        public static double Error9 => 0.000000001;
        public static double Error10 => 0.0000000001;
        /// <summary>
        /// Выделение памяти 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="N">размер массива</param>
        /// <param name="X">массив</param>
        public static void Alloc<T>(int N, ref T[] X)
        {
            if (X == null)
                X = new T[N];
            else
                if (X.Length != N)
                X = new T[N];
        }
        /// <summary>
        /// Выделение памяти с присвоением значения
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="N">размер массива</param>
        /// <param name="X">массив</param>
        /// <param name="value">значение</param>
        public static void Alloc<T>(int N, ref T[] X, T value)
        {
            if (X == null)
                X = new T[N];
            else
                if (X.Length != N)
                X = new T[N];
            for (int i = 0; i < N; i++)
                X[i] = value;
        }
        /// <summary>
        /// Очистка/установка массива
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="value"></param>
        public static void MemSet<T>(this T[] arr, T value)
        {
            for (int i = 0; i < arr.Length; i++)
                arr[i] = value;
        }
        /// <summary>
        /// Выделение памяти или очистка квадратной матрицы
        /// </summary>
        /// <param name="Count">размерность</param>
        /// <param name="LaplMatrix">матрица</param>
        public static void Alloc2DClear(int Count, ref double[][] LaplMatrix)
        {
            if (LaplMatrix == null)
            {
                LaplMatrix = new double[Count][];
                for (int i = 0; i < Count; i++)
                    LaplMatrix[i] = new double[Count];
            }
            else
            {
                if (Count == LaplMatrix.Length)
                {
                    for (int i = 0; i < Count; i++)
                    {
                        for (int j = 0; j < Count; j++)
                            LaplMatrix[i][j] = 0;
                    }
                }
                else
                {
                    LaplMatrix = new double[Count][];
                    for (int i = 0; i < Count; i++)
                        LaplMatrix[i] = new double[Count];
                }
            }
        }
        public static void Alloc2DClear(uint Count, ref double[][] LaplMatrix)
        {
            if (LaplMatrix == null)
            {
                LaplMatrix = new double[Count][];
                for (int i = 0; i < Count; i++)
                    LaplMatrix[i] = new double[Count];
            }
            else
            {
                if (Count == LaplMatrix.Length)
                {
                    for (int i = 0; i < Count; i++)
                    {
                        for (int j = 0; j < Count; j++)
                            LaplMatrix[i][j] = 0;
                    }
                }
                else
                {
                    LaplMatrix = new double[Count][];
                    for (int i = 0; i < Count; i++)
                        LaplMatrix[i] = new double[Count];
                }
            }
        }

        /// <summary>
        /// Выделение памяти или очистка квадратной матрицы
        /// </summary>
        /// <param name="Nx">размерность</param>
        /// <param name="Ny">размерность</param>
        /// <param name="LaplMatrix">матрица</param>
        public static void Alloc2DClear(int Nx, int Ny, ref double[][] LaplMatrix)
        {
            if (LaplMatrix == null)
            {
                LaplMatrix = new double[Nx][];
                for (int i = 0; i < Nx; i++)
                    LaplMatrix[i] = new double[Ny];
            }
            else
            {
                if (Nx == LaplMatrix.Length)
                {
                    if (Ny == LaplMatrix[0].Length)
                    {
                        for (int i = 0; i < Nx; i++)
                            for (int j = 0; j < Ny; j++)
                                LaplMatrix[i][j] = 0;
                    }
                    else
                    {
                        for (int i = 0; i < Nx; i++)
                            LaplMatrix[i] = new double[Ny];
                    }
                }
                else
                {
                    LaplMatrix = new double[Nx][];
                    for (int i = 0; i < Nx; i++)
                        LaplMatrix[i] = new double[Ny];
                }
            }
        }
        public static void Alloc2DClear(uint Nx, uint Ny, ref double[][] LaplMatrix)
        {
            if (LaplMatrix == null)
            {
                LaplMatrix = new double[Nx][];
                for (int i = 0; i < Nx; i++)
                    LaplMatrix[i] = new double[Ny];
            }
            else
            {
                if (Nx == LaplMatrix.Length)
                {
                    if (Ny == LaplMatrix[0].Length)
                    {
                        for (int i = 0; i < Nx; i++)
                            for (int j = 0; j < Ny; j++)
                                LaplMatrix[i][j] = 0;
                    }
                    else
                    {
                        for (int i = 0; i < Nx; i++)
                            LaplMatrix[i] = new double[Ny];
                    }
                }
                else
                {
                    LaplMatrix = new double[Nx][];
                    for (int i = 0; i < Nx; i++)
                        LaplMatrix[i] = new double[Ny];
                }
            }
        }
        /// <summary>
        /// Выделение памяти или очистка массива
        /// </summary>
        /// <param name="N">размерность</param>
        /// <param name="X">массив</param>
        public static void AllocClear(int N, ref double[] X)
        {
            if (X == null)
                X = new double[N];
            else
            {
                if (X.Length != N)
                    X = new double[N];
                else
                    for (int i = 0; i < N; i++)
                        X[i] = 0;
            }
        }
        public static void MemCpy<T>(T[] a, T[] b, int Count = 0)
        {
            if (Count == 0 || Count != b.Length)
                Count = b.Length;
            if (a == null)
                a = new T[Count];
            if (a.Length < Count)
                a = new T[Count];
            for (int i = 0; i < Count; i++)
                a[i] = b[i];
        }
    }
}
