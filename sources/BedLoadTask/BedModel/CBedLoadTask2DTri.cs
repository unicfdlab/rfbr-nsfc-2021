﻿//---------------------------------------------------------------------------
//                 Реализация библиотеки для моделирования 
//                  гидродинамических и русловых процессов
//---------------------------------------------------------------------------
//                Модуль BLLib для расчета донных деформаций 
//                 (учет движения только влекомых наносов)
//         по русловой модели Петрова А.Г. и Потапова И.И. от 2014 г.
//                  (добавлены более раннии модели < 2014 г.)
//                              Потапов И.И.
//                               27.02.21
//---------------------------------------------------------------------------
namespace BedLoadTask.BedModel
{
    using BedLoadTask.Algebra;
    using BedLoadTask.Log;
    using BedLoadTask.Mesh;
    using BedLoadTask.SaveData;
    using System;
    using System.Linq;
    /// <summary>
    /// ОО: Класс для решения плановой задачи о 
    /// расчете донных деформаций русла на симплекс сетке
    /// </summary>
    [Serializable]
    public class CBedLoadTask2DTri : ABedLoadTask2D
    {
        /// <summary>
        /// Лавинный решатель
        /// </summary>
        //Avalanche2D avalanche;
        Avalanche2DCircle avalanche;
        ///// <summary>
        ///// Количество узлов на КЭ
        ///// </summary>
        
        /// <summary>
        /// Поток наносов по Х в улах
        /// </summary>
        double[] Gx;
        /// <summary>
        /// Поток наносов по У в улах
        /// </summary>
        double[] Gy;
        /// <summary>
        /// Площадь КО связанного с узлом
        /// </summary>
        double[] Sknot;
        /// <summary>
        /// Поток наносов по Х на элементе
        /// </summary>
        double[] GxE;
        /// <summary>
        /// Поток наносов по У на элементе
        /// </summary>
        double[] GyE;
        /// <summary>
        /// Плошадь КЭ
        /// </summary>
        double S = 0;
        /// <summary>
        /// Касательные напряжения по Х в узлах КЭ
        /// </summary>
        double[] Tx = { 0, 0, 0 };
        /// <summary>
        /// Касательные напряжения по У в узлах КЭ
        /// </summary>
        double[] Ty = { 0, 0, 0 };
        /// <summary>
        /// ненормированные производные от функций формы по х
        /// </summary>
        double[] a = { 0, 0, 0 };
        /// <summary>
        /// ненормированные производные от функций формы по у
        /// </summary>
        double[] b = { 0, 0, 0 };
        /// <summary>
        /// Придонное давление в узлах КЭ
        /// </summary>
        double[] epress = { 0, 0, 0 };
        /// <summary>
        /// Отметки дна в узлах КЭ
        /// </summary>
        double[] ezeta = { 0, 0, 0 };
        /// <summary>
        /// Матрица донной подвижности для  КЭ
        /// </summary>
        double[,] BB = new double[2, 2];
        /// <summary>
        /// Матрица давления для  КЭ
        /// </summary>
        double[,] H = new double[2, 2];
        /// <summary>
        /// Матрица масс для КЭ - каркас
        /// </summary>
        double[,] MM = { { 2, 1, 1 }, { 1, 2, 1 }, { 1, 1, 2 } };
        /// <summary>
        /// Флаг для определения сухого-мокрого дна
        /// </summary>
        public int[] DryWet = null;
        /// <summary>
        /// Модуль вектора касательных напряжений в узлах
        /// </summary>
        public double[] Tau = null;
        ///// <summary>
        ///// Погрешность при вычислении коэффициентов
        ///// </summary>
        //double ErrAE = 0.000000001;
        // локальные переменные
        double GC, MatrixK, MatrixM;
        double dZetadX, dZetadY, dPX, dPY;
        double cosA, sinA;
        double cos2, sin2, cs2;
        double ss;
        /// <summary>
        /// Конструктор 
        /// </summary>
        public CBedLoadTask2DTri(BedLoadParams p) : base(p)
        {
            cu = 3;
            ss = (1 + s) / s;
        }
        /// <summary>
        /// Установка исходных данных
        /// </summary>
        /// <param name="mesh">Сетка расчетной области</param>
        /// <param name="algebra">Решатель для СЛАУ </param>
        /// <param name="BCBed">граничные условия</param>
        /// <param name="Zeta0">начальный уровень дна</param>
        /// <param name="theta">Параметр схемы по времени</param>
        /// <param name="dt">шаг по времени</param>
        /// <param name="isAvalanche">флаг использования лавинной модели</param>
        public override void SetTask(IMesh mesh, IAlgebra algebra, BCondition BCBed, TypeBLModel blm, double[] Zeta0, double theta, double dt)
        {
            cu = 3;
            double Relax = 0.5;
            uint CountAvalanche = 25;
            uint NN = (uint)Math.Sqrt(mesh.Count);

            base.SetTask(mesh, algebra, BCBed, blm, Zeta0, theta, dt);
            //avalanche = new Avalanche2D(mesh, tanphi, Relax, avalancheCount);
            avalanche = new Avalanche2DCircle(mesh, 6 * (uint)mesh.Count, NN, CountAvalanche, tanphi, Relax);

            DryWet = new int[mesh.Count];
            Tau = new double[mesh.Count];
            Gx = new double[mesh.Count];
            Gy = new double[mesh.Count];
            Sknot = new double[mesh.Count];
            GxE = new double[mesh.CountElement];
            GyE = new double[mesh.CountElement];
        }

        /// <summary>
        /// Вычисление изменений формы донной поверхности 
        /// на одном шаге по времени по модели 
        /// Петрова А.Г. и Потапова И.И. 2014
        /// Реализация решателя - методом контрольных объемов,
        /// Патанкар (неявный дискретный аналог ст 40 ф.3.40 - 3.41)
        /// Коэффициенты донной подвижности, определяются 
        /// как среднее гармонические величины         
        /// </summary>
        /// <param name="Zeta0">текущая форма дна</param>
        /// <param name="tau">придонное касательное напряжение</param>
        /// <param name="P">придонное давление</param>
        /// <returns>новая форма дна</returns>
        /// </summary>
        public override void CalkZetaFDM(ref double[] Zeta, double[] tauX, double[] tauY, double[] P = null)
        {
            try
            {
                if (mesh == null) return;
                MEM.Alloc<double>(Zeta0.Length, ref Zeta);
                MEM.Alloc<double>(Zeta0.Length, ref MRight);
                algebra.Clear();
                Ralgebra.Clear();

                for (int i = 0; i < Sknot.Length; i++)
                {
                    Sknot[i] = 0;
                    Gx[i] = 0;
                    Gy[i] = 0;
                }

                for (uint elem = 0; elem < mesh.CountElement; elem++)
                {
                    InitLocal(cu);
                    // получить узлы КЭ
                    mesh.ElementKnots(elem, ref knots);
                    // координаты и площадь
                    mesh.ElemX(elem, ref x);
                    mesh.ElemY(elem, ref y);
                    // площадь
                    S = mesh.ElemSquare(x, y);
                    if (S == 0)
                    {
                        Console.WriteLine("elem = {0}", elem);
                    }
                    a[0] = y[1] - y[2];
                    b[0] = x[2] - x[1];
                    a[1] = y[2] - y[0];
                    b[1] = x[0] - x[2];
                    a[2] = y[0] - y[1];
                    b[2] = x[1] - x[0];

                    // определение градиента дна по х,у
                    mesh.ElemValues(Zeta0, elem, ref ezeta);

                    dZetadX = 0;
                    dZetadY = 0;
                    for (int ai = 0; ai < cu; ai++)
                    {
                        dZetadX += a[ai] * ezeta[ai];
                        dZetadY += b[ai] * ezeta[ai];
                    }
                    dZetadX /= (2 * S);
                    dZetadY /= (2 * S);
                    CosGamma[elem] = 1.0 / Math.Sqrt(1 + dZetadX * dZetadX + dZetadY * dZetadY);
                    double mTx = 0;
                    double mTy = 0;
                    // Подготовка коэффициетов
                    if (tauX.Length == mesh.CountElement)
                    {
                        // напряжения заданны на элементе
                        tau[elem] = Math.Sqrt(tauX[elem] * tauX[elem] + tauY[elem] * tauY[elem]);
                        if (tau[elem] > eZeta)
                        {
                            cosA = tauX[elem] / tau[elem];
                            sinA = tauY[elem] / tau[elem];
                        }
                        else
                        {
                            cosA = 1; sinA = 0;
                        }
                    }
                    else
                    {
                        // напряжения заданны в узлах
                        mesh.ElemValues(tauX, elem, ref Tx);
                        mesh.ElemValues(tauY, elem, ref Ty);
                        mTx = Tx.Sum() / 3;
                        mTy = Ty.Sum() / 3;
                        tau[elem] = Math.Sqrt(mTx * mTx + mTy * mTy);
                        if (tau[elem] > eZeta)
                        {
                            cosA = mTx / (tau[elem]);
                            sinA = mTy / (tau[elem]);
                        }
                        else
                        {
                            cosA = 1; sinA = 0;
                        }
                    }

                    //cosA = 1; sinA = 0;
                    cos2 = cosA * cosA;
                    sin2 = sinA * sinA;
                    cs2 = sinA * cosA;
                    double mtauS = mTx * cosA + mTy * sinA;
                    mtau = tau[elem];

                    GC = rho_s * G1 * mtauS * Math.Sqrt(mtau) / CosGamma[elem];


                    if (tau0 > mtau)
                        chi = 1;
                    else
                        chi = Math.Sqrt(tau0 / mtau);

                    if (blm != TypeBLModel.BLModel_1991)
                    {
                        // определение градиента давления по х,у
                        mesh.ElemValues(P, elem, ref epress);
                        dPX = 0;
                        dPY = 0;
                        for (int ai = 0; ai < cu; ai++)
                        {
                            dPX += a[ai] * epress[ai] / (rho_b * g);
                            dPY += b[ai] * epress[ai] / (rho_b * g);
                        }

                        if (blm != TypeBLModel.BLModel_2010)
                        {
                            A[elem] = Math.Max(0, 1 - chi);
                            B[elem] = (chi / 2 + A[elem]) / tanphi;
                            C[elem] = A[elem] / (s * tanphi);
                        }
                        else
                        {
                            A[elem] = Math.Max(0, 1 - chi);
                            B[elem] = (chi / 2 + A[elem] * (1 + s) / s) / tanphi;
                            C[elem] = A[elem] / (s * tanphi);
                        }
                    }
                    else
                    {
                        A[elem] = Math.Max(0, 1 - chi);
                        B[elem] = (chi / 2 + A[elem]) / tanphi;
                        C[elem] = 0;
                        ss = 1;
                    }
                    D[elem] = 4.0 / 5.0 / tanphi;

                    #region добаление диффузионной лавинной модели от 23 11 2021 
                    if (A[elem] < MEM.Error10)
                    {
                        // тангенс угла
                        double tanGamma = Math.Sqrt(1 - CosGamma[elem] * CosGamma[elem]) / CosGamma[elem];
                        // лавинная диффузия
                        double AvalancheDiff = Math.Max(0, tanGamma - tanphi);
                        if (AvalancheDiff < MEM.Error5 ||
                            isAvalanche == AvalancheType.NonAvalanche ||
                            isAvalanche == AvalancheType.AvalancheQuad_2021)
                        {
                            B[elem] = 0;
                            D[elem] = 0;
                        }
                        else
                        {
                            double Currant = 0.1;
                            B[elem] = Currant * dt * AvalancheDiff * B[elem];
                            D[elem] = Currant * dt * AvalancheDiff * D[elem];
                        }
                    }
                    #endregion 

                    BB[0, 0] = ss * D[elem] * sin2 + B[elem] * cos2;
                    BB[0, 1] = cs2 * (B[elem] - ss * D[elem]);
                    BB[1, 0] = cs2 * (B[elem] - ss * D[elem]);
                    BB[1, 1] = ss * D[elem] * cos2 + B[elem] * sin2;
                    // ?
                    H[0, 0] = -D[elem] / s * sin2 - C[elem] * cos2;
                    H[0, 1] = cs2 * (D[elem] / s - C[elem]);
                    H[1, 0] = cs2 * (D[elem] / s - C[elem]);
                    H[1, 1] = -D[elem] / s * cos2 - C[elem] * sin2;

                    #region Вычисление потоков на элементе и их разброс в узлы
                    GxE[elem] = GC * (A[elem] * cosA - BB[0, 0] * dZetadX - BB[0, 1] * dZetadY);
                    GyE[elem] = GC * (A[elem] * sinA - BB[1, 0] * dZetadX - BB[1, 1] * dZetadY);
                    Sknot[knots[0]] += S / 3;
                    Sknot[knots[1]] += S / 3;
                    Sknot[knots[2]] += S / 3;
                    Gx[knots[0]] += GxE[elem] * S / 3;
                    Gx[knots[1]] += GxE[elem] * S / 3;
                    Gx[knots[2]] += GxE[elem] * S / 3;
                    Gy[knots[0]] += GyE[elem] * S / 3;
                    Gy[knots[1]] += GyE[elem] * S / 3;
                    Gy[knots[2]] += GyE[elem] * S / 3;
                    #endregion

                    // вычисление ЛЖМ
                    for (int ai = 0; ai < cu; ai++)
                        for (int aj = 0; aj < cu; aj++)
                        {
                            // Матрица масс
                            MatrixM = MM[ai, aj] * S / 12;
                            // Матрица жесткости
                            MatrixK = GC * (BB[0, 0] * a[ai] * a[aj] +
                                            BB[0, 1] * a[ai] * b[aj] +
                                            BB[1, 0] * b[ai] * a[aj] +
                                            BB[1, 1] * b[ai] * b[aj]) / (4 * S);
                            // Левая матрица n+1 слоя по времени
                            LaplMatrix[ai][aj] = MatrixM / dt + theta * MatrixK;
                            // Правая матрица n слоя по времени
                            RMatrix[ai][aj] = MatrixM / dt - (1 - theta) * MatrixK;
                        }

                    // учет транзитного расхода
                    for (int ai = 0; ai < cu; ai++)
                        LocalRight[ai] = GC * A[elem] * (a[ai] * cosA + b[ai] * sinA) / 2;

                    // учет градиента придонного давления
                    if (blm != TypeBLModel.BLModel_1991)
                        for (int ai = 0; ai < cu; ai++)
                            LocalRight[ai] += GC * (a[ai] * (H[0, 0] * dPX + H[0, 1] * dPY) +
                                                       b[ai] * (H[1, 0] * dPX + H[1, 1] * dPY)) / (12 * S);

                    // добавление вновь сформированной ЛЖМ в ГМЖ
                    algebra.AddToMatrix(LaplMatrix, knots);
                    // добавление вновь сформированной ЛЖМ в ГМЖ
                    Ralgebra.AddToMatrix(RMatrix, knots);
                    // добавление вновь сформированной ЛПЧ в ГПЧ
                    algebra.AddToRight(LocalRight, knots);
                }

                for (int i = 0; i < Sknot.Length; i++)
                {
                    Gx[i] = Gx[i] / Sknot[i];
                    Gy[i] = Gy[i] / Sknot[i];
                }

                #region отладка + ГУ
                //((SparseAlgebra)Ralgebra).CheckSYS("1 Ralgebra");
                //((SparseAlgebra)algebra).CheckSYS("1 algebra");
                //for (int bi = 0; bi < 4; bi++)
                //{
                //    uint[] bound1 = mesh.BoundKnotsByType(bi);
                //    double[] TL = new double[bound1.Length];
                //    for (int i = 0; i < TL.Length; i++)
                //        TL[i] = Zeta0[bound1[i]];
                //    Ralgebra.BoundConditions(TL, bound1);
                //}
                //uint[] bound3 = mesh.BoundKnotsByType(3);
                //double[] TR = new double[bound3.Length];
                //for (int i = 0; i < TR.Length; i++)
                //    TR[i] = Zeta0[bound3[i]];
                //Ralgebra.BoundConditions(TL, bound1);
                //Ralgebra.BoundConditions(TR, bound3);
                //if (CountDryWet > 0)
                //    Ralgebra.BoundConditions(DryWetZeta, DryWetNumbers);
                //((SparseAlgebra)Ralgebra).CheckSYS("2 Ralgebra после ГУ");
                //((SparseAlgebra)algebra).CheckMas(Zeta0, "Zeta0");
                #endregion

                Ralgebra.getResidual(ref MRight, Zeta0, 0);
#if DEBUG                
                ((SparseAlgebra)algebra).CheckMas(MRight, "MRight");
                ((SparseAlgebra)algebra).CheckSYS("2 algebra");
#endif
                algebra.CopyRight(MRight);
#if DEBUG
                #region отладка

                ((SparseAlgebra)algebra).CheckSYS("3 algebra");

                ////Удовлетворение ГУ
                //for (int bi = 0; bi < 4; bi++)
                //{
                //    uint[] bound1 = mesh.BoundKnotsByType(bi);
                //    double[] TL = new double[bound1.Length];
                //    for (int i = 0; i < TL.Length; i++)
                //        TL[i] = Zeta0[bound1[i]];
                //    algebra.BoundConditions(TL, bound1);
                //}
                //if (CountDryWet > 0)
                //    algebra.BoundConditions(DryWetZeta, DryWetNumbers);
                ((SparseAlgebra)algebra).CheckSYS("algebra 4");

                #endregion
#endif

                algebra.Solve(ref Zeta);

                // Сглаживание дна по лавинной моделе
                if (isAvalanche == AvalancheType.AvalancheQuad_2021)
                    avalanche.Lavina(ref Zeta);

                // переопределение начального значения ezeta 
                // для следующего шага по времени
                for (int j = 0; j < Zeta.Length; j++)
                    Zeta0[j] = Zeta[j];
            }
            catch (Exception e)
            {
                Message = e.Message;
                for (int j = 0; j < Zeta.Length; j++)
                    Zeta[j] = Zeta0[j];
            }
        }
        /// <summary>
        /// Формирование данных для отрисовки связанных с сеткой IMesh со сороны задачи
        /// </summary>
        /// <param name="sp">контейнер данных</param>
        public override void AddMeshPolesForGraphics(ISavePoint sp)
        {
            if (sp != null)
            {
                double[] dw = new double[DryWet.Length];
                for (int i = 0; i < dw.Length; i++)
                    dw[i] = DryWet[i];
                sp.Add("DryWet", dw);
                sp.Add("Поток Gx", Gx);
                sp.Add("Поток Gy", Gy);
                sp.Add("Поток G", Gx, Gy);
            }
        }
    }
}