/// <summary>
///     Срез библиотек 
///     BBLib,
///     AlgebraLib,
///     MeshLib,
///     RenderLib,
///     от 05.05.2021 для проекта 
///     Потапов И.И.
///     30.06.21
///   c 01 08 21 в основной ветке проекта изменены интерфейсы задач
///   c 23 1 21  в изменены методы лавинных моделей
/// </summary>
namespace BedLoadTask
{
    using BedLoadTask.Algebra;
    using BedLoadTask.BedModel;
    using BedLoadTask.Log;
    using BedLoadTask.Mesh;
    using BedLoadTask.MeshAdapter;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    class Start
    {
        static bool test = false;
        static void Main(string[] args)
        {
            if(test == false)
                Run(args, 0);
            else
            {
                for (int i = 0; i < 100; i++)
                    Run(args, i, test);
            }
        }

        static void Run(string[] args, int indedx, bool test = false)
        {
            //LOG.LogMessage(" Версия от 26 08 2021 расширены параметры командной строки заданны ");
            LOG.LogMessage(" Version 01 09 2021 extended command line parameters are given ");
            // установка параметров задачи из гридов в задачу
            ExBedLoadParams exblp = new ExBedLoadParams();
            BedLoadParams blp = new BedLoadParams();
            double theta = 0.5;
            double dt = 0.1;
            string openFileName =  "ImportData.txt";
            string saveFileName =  "ExportData.txt";
            string saveMFileName = "MExportData.txt";
            string NameBLParams =  "BLParams.txt";
            IAlgebra algebra = null;

            if (args.Length >= 5)
            {
                if (args.Length == 1)
                {
                    NumberFormatInfo formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
                    LOG.LogMessage(" Параметры командной строки заданны ");
                    LOG.LogMessage(" Command line parameters specified ");
                    dt = double.Parse(args[1].Trim(), formatter);
                }
                if (args.Length >= 5)
                {
                    NumberFormatInfo formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
                    LOG.LogMessage(" Параметры командной строки заданны ");
                    LOG.LogMessage(" Command line parameters specified ");
                    theta = double.Parse(args[0].Trim(), formatter);
                    dt = double.Parse(args[1].Trim(), formatter);
                    openFileName = args[2].Trim();
                    saveFileName = args[3].Trim();
                    NameBLParams = args[4].Trim();
                }
            }
            try
            {
                StreamReader file = new StreamReader(NameBLParams);
                exblp.Load(file);
                blp = exblp;
                LOG.LogMessage("Файл парамеров задачи - доные деформации " + NameBLParams + " загружен");
                LOG.LogMessage("The file of task parameters - bottom deformations " + NameBLParams + " is loaded");
            }
            catch (Exception mes)
            {
                LOG.LogMessage("Файл парамеров задачи - доные деформации " + NameBLParams + " не обнаружен");
                LOG.LogMessage("File of task parameters - bottom deformations " + NameBLParams + " not found");
                LOG.LogMessage("Будут использованы парамеров задачи - доные деформации по умолчанию");
                LOG.LogMessage("The task parameters will be used - default bottom deformations");
                LOG.LogMessage(mes.Message);
            }
            TriMesh mesh = null;
            double[] Zeta0 = null;
            double[] Zeta = null;
            double[] tauX = null;
            double[] tauY = null;
            double[] P = null;
            MeshAdapter2D ma = new MeshAdapter2D();
            // если расширение отсутсвует грузим нерегулярную сетку и напряжения
            if (exblp.IsExtension == false)
            {
                try
                {
                    // LOG.LogMessage("Чтение файла нагрузки для задачи доные деформации " + openFileName);
                    LOG.LogMessage("Reading the load file for the problem of bottom deformations " + openFileName);
                    ma.LoadData(openFileName, ref mesh, ref Zeta0, ref tauX, ref tauY, ref P);
                    double minTauX = tauX.Min();
                    double maxTauX = tauX.Max();
                    if (Math.Abs(maxTauX) > Math.Abs(minTauX))
                    {
                        // LOG.LogMessage("Поля напряжений не требуют инверсии");
                        LOG.LogMessage("Stress fields do not require inversion");
                        for (int i = 0; i < Zeta0.Length; i++)
                        {
                            tauX[i] = tauX[i];
                            tauY[i] = tauY[i];
                        }
                    }
                    else
                    {
                        // LOG.LogMessage("Для полей напряжений выполнена инверсия");
                        LOG.LogMessage("The inversion is performed for stress fields");
                        for (int i = 0; i < Zeta0.Length; i++)
                        {
                            tauX[i] = -tauX[i];
                            tauY[i] = -tauY[i];
                        }
                    }
                    LOG.LogMessage("Файл нагрузки для задачи доные деформации " + openFileName + " загружен");
                    LOG.LogMessage("Load file for the problem bottom deformations " + openFileName + " is loaded");
                }
                catch (Exception mes)
                {
                    LOG.LogMessage(" Файл нагрузки " + openFileName + " для задачи расчета дна загружен не корректно");
                    LOG.LogMessage("Load file" + openFileName + "for the problem of bottom calculation is not loaded correctly");
                    LOG.LogMessage(mes.Message);
                    return;
                }
            }
            else
            {
                // Решение задачи ищем в прямоугольной области, на регулярной прямоугольной сетке 
                ma.GetRectsngleMesh(ref mesh, exblp.Nx, exblp.Ny, exblp.dx, exblp.dy);
                LOG.LoadMasLine(exblp.FNameTauX, ref tauX, mesh.Count);
                LOG.LoadMasLine(exblp.FNameTauY, ref tauY, mesh.Count);
                LOG.LoadMasLine(exblp.FNameTauY, ref P, mesh.Count);
                LOG.LoadMasLine(exblp.FNameTauY, ref Zeta0, mesh.Count);
                //ma.SaveData("test.txt", mesh, Zeta0, tauX, tauY, P);
            }
            LOG.LogMessage(" Файл данных для задачи расчета дна загружен ");
            LOG.LogMessage(" the data file for the problem of bottom calculation is loaded ");

            BCondition BCBed = new BCondition(TypeBC.Dirichlet, TypeBC.Neumann, 1, 1);
            // тип модели
            TypeBLModel blm = TypeBLModel.BLModel_1991;
            // Задача плановых деформаций дна
            CBedLoadTask2DTri BedLoadTask = new CBedLoadTask2DTri(blp);
            BedLoadTask.SetTask(mesh, algebra, BCBed, blm, Zeta0, theta, dt);
            // вычисление донных деформаций
            BedLoadTask.CalkZetaFDM(ref Zeta, tauX, tauY, null);
            LOG.LogMessage(" расчет задачи донных деформаций завершен ");
            LOG.LogMessage(" calculation of the problem of bottom deformations completed ");
            // сохранение донных деформаций
            // добавлен циклический тест 25 11 2021
            if (exblp.IsExtension == false)
            {
                if(test == false)
                    ma.SaveData(saveMFileName, mesh, Zeta, tauX, tauY, P);
                else
                {
                    string Mane = "MExportData" + indedx.ToString() + ".txt";
                    ma.SaveData(openFileName, mesh, Zeta, tauX, tauY, P);
                    ma.SaveData(Mane, mesh, Zeta, tauX, tauY, P);
                }
                ma.SaveDataZeta(saveFileName, mesh, Zeta);
            }
            else
            {
                ma.SaveData(saveMFileName, mesh, Zeta, tauX, tauY, P);
                LOG.SaveMasLine(exblp.FNameZeta, Zeta, 6);
                LOG.SaveMasLine(exblp.FNameX, mesh.GetCoordsX(), 6);
                LOG.SaveMasLine(exblp.FNameY, mesh.GetCoordsY(), 6);
            }
            LOG.LogMessage(" данные по деформируемому дну выгружены в файл ");
        }
    }
}
