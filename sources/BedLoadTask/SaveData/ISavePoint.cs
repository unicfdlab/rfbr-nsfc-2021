//---------------------------------------------------------------------------
//                    ПРОЕКТ  "РУСЛОВЫЕ ПРОЦЕССЫ"
//                         проектировщик:
//                           Потапов И.И.
//---------------------------------------------------------------------------
//                 кодировка : 14.12.2020 Потапов И.И.
//---------------------------------------------------------------------------
namespace BedLoadTask.SaveData
{
    /// <summary>
    /// Адаптер точки сохранения
    /// </summary>
    public interface ISavePoint
    {
        /// <summary>
        /// Добавление поля привязанного к узлам сетки
        /// </summary>
        /// <param name="Name">Название поля</param>
        /// <param name="Value">Значение поля в узлах сетки</param>
        void Add(string Name, double[] Value);
        /// <summary>
        /// Добавление векторного поля привязанного к узлам сетки
        /// </summary>
        /// <param name="Name">Название поля</param>
        /// <param name="Vx">Аргументы функции поля</param>
        /// <param name="Vy">Значение функции поля</param>
        void Add(string Name, double[] Vx, double[] Vy);
        /// <summary>
        /// Добавление поля не привязанного к узлам сетки
        /// </summary>
        /// <param name="Name">Название поля</param>
        /// <param name="arg">Аргументы функции поля</param>
        /// <param name="Value">Значение функции поля</param>
        void AddСurve(string Name, double[] X, double[] Y);
        /// <summary>
        /// Сериализация sp
        /// </summary>
        /// <param name="sp">точка сохранения</param>
        /// <param name="NameSave">путь,имя файла</param>
        void SerializableSavePoint(ISavePoint sp, string NameSave);
        /// <summary>
        /// Загрузка точки сохранения
        /// </summary>
        /// <param name="FileName">путь,имя файла</param>
        /// <returns></returns>
        ISavePoint LoadSavePoint(string FileName);
    }
}