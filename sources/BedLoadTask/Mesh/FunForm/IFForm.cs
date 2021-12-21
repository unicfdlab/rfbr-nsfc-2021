//---------------------------------------------------------------------------
//                       ПРОЕКТ  "RiverLib"
//                         проектировщик:
//                           Потапов И.И.
//---------------------------------------------------------------------------
//                 правка  :   04.03.2021 Потапов И.И.
//---------------------------------------------------------------------------
namespace BedLoadTask.Mesh.FunForm
{
    /// <summary>
    /// ID функций формы, представленных в библиотеке
    /// </summary>
    public enum TypeFunForm
    {
        Form_1D_L0 = 0,
        Form_1D_L1,
        Form_1D_L2,
        Form_1D_L3,
        Form_2D_TriangleAnalitic_L1,
        Form_2D_Triangle_L0,
        Form_2D_Triangle_L1,
        Form_2D_Triangle_CR,
        Form_2D_Triangle_L2,
        Form_2D_Triangle_L3,
        Form_2D_Rectangle_L0,
        Form_2D_Rectangle_L1,
        Form_2D_Rectangle_S2,
        Form_2D_Rectangle_S3,
        Form_2D_Rectangle_L2,
        Form_2D_Rectangle_L3,
        Form_2D_Rectangle_CR,
        Form_2D_Rectangle_P,
        Form_2D_Rectangle_Area4_L1N,
        Form_2D_Rectangle_Area3_L1N,
        Form_2D_Rectangle_CRM
    }
    /// <summary>
    /// Порядок КЭ сетки на которой определяется функция формы
    /// </summary>
    public enum IntRange
    {
        intRange1 = 1,
        intRange2,
        intRange3,
        intRange4
    }
    /// <summary>
    /// ОО:  Функции формы КЭ
    /// </summary>
    public interface IFForm
    {
        /// <summary>
        /// Рекомендуемый порядок интегрирования функции формы
        /// </summary>
        IntRange MeshRange { get; }
        /// <summary>
        /// идентификатор функции формы
        /// </summary>
        TypeFunForm ID { get; }
        /// <summary>
        /// вычисление значений функций формы
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        void CalkForm(double x, double y);
        /// <summary>
        /// вычисление значений функций формы ее производных
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        void CalkDiffForm(double x, double y);
        /// <summary>
        /// вычисление  координат i узла
        /// </summary>
        /// <param name="IdxKnot"></param>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        void CalkVertex(uint IdxKnot, ref double _x, ref double _y);
        /// <summary>
        /// вычисление локальных производных функций формы
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="BWM">ограничители</param>
        void CalkDiffForm(double x, double y, double[,] BWM = null);
        /// <summary>
        /// Тип функций формы на гранях КЭ
        /// </summary>
        /// <returns></returns>
        uint GetBoundFormType();
    }
}

