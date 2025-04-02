using OfficeOpenXml;
using System;
using System.Collections.Generic;
//using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Wpf_Inventory_.Model;
using Microsoft.EntityFrameworkCore;

namespace Wpf_Inventory_.Classes
{
    internal class ExcelCommunication
    {
        // Метод для импорта данных из Excel
        public static void ImportFromExcel(string filePath, InventoryDataBaseContext dbContext)
        {
            FileInfo fileInfo = new FileInfo(filePath);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(fileInfo))
            {
                var worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++) // Предполагаем, что первая строка - заголовки
                {
                    try
                    {
                        var device = new Device
                        {
                            DeviceName = worksheet.Cells[row, 1].Text,
                            IpAddress = worksheet.Cells[row, 2].Text,
                            SerialNumber = worksheet.Cells[row, 3].Text,
                            InventoryNumber = worksheet.Cells[row, 4].Text,
                            Note = worksheet.Cells[row, 5].Text,
                            DateOfCommissioning = DateTime.Parse(worksheet.Cells[row, 6].Text),
                            Exception = worksheet.Cells[row, 7].Text.ToLower() == "да",

                            // Обработка связей
                            Department = GetOrCreateDepartment(dbContext, worksheet.Cells[row, 8].Text),
                            Devicetype = GetOrCreateDeviceType(dbContext, worksheet.Cells[row, 9].Text),
                            Model = GetOrCreateModel(dbContext, worksheet.Cells[row, 10].Text),
                            Office = GetOrCreateOffice(dbContext, worksheet.Cells[row, 11].Text)
                        };

                        // Проверка на дубликаты
                        if (!dbContext.Device.Any(d => d.InventoryNumber == device.InventoryNumber))
                        {
                            dbContext.Device.Add(device);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка в строке {row}: {ex.Message}");
                    }
                }

                dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// Метод для экспорта данных в Excel
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="dbContext"></param>
        public static void ExportToExcel(string filePath, InventoryDataBaseContext dbContext)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Devices");

                // Заголовки
                worksheet.Cells[1, 1].Value = "Название устройства";
                worksheet.Cells[1, 2].Value = "IP-адрес";
                worksheet.Cells[1, 3].Value = "Серийный номер";
                worksheet.Cells[1, 4].Value = "Инвентарный номер";
                worksheet.Cells[1, 5].Value = "Примечание";
                worksheet.Cells[1, 6].Value = "Дата ввода в эксплуатацию";
                worksheet.Cells[1, 7].Value = "Исключение";
                worksheet.Cells[1, 8].Value = "Отдел";
                worksheet.Cells[1, 9].Value = "Тип устройства";
                worksheet.Cells[1, 10].Value = "Модель";
                worksheet.Cells[1, 11].Value = "Офис";

                // Данные
                var devices = dbContext.Device
                    .Include(d => d.Department)
                    .Include(d => d.Devicetype)
                    .Include(d => d.Model)
                    .Include(d => d.Office)
                    .ToList();

                for (int i = 0; i < devices.Count; i++)
                {
                    var device = devices[i];
                    worksheet.Cells[i + 2, 1].Value = device.DeviceName;
                    worksheet.Cells[i + 2, 2].Value = device.IpAddress;
                    worksheet.Cells[i + 2, 3].Value = device.SerialNumber;
                    worksheet.Cells[i + 2, 4].Value = device.InventoryNumber;
                    worksheet.Cells[i + 2, 5].Value = device.Note;
                    worksheet.Cells[i + 2, 6].Value = device.DateOfCommissioning?.ToString("dd.MM.yyyy");
                    worksheet.Cells[i + 2, 7].Value = device.Exception.HasValue && device.Exception.Value ? "Да" : "Нет";
                    worksheet.Cells[i + 2, 8].Value = device.Department?.Name;
                    worksheet.Cells[i + 2, 9].Value = device.Devicetype?.Type;
                    worksheet.Cells[i + 2, 10].Value = device.Model?.Model1;
                    worksheet.Cells[i + 2, 11].Value = device.Office?.Officenum;
                }

                // Сохранение файла
                FileInfo excelFile = new FileInfo(filePath);
                package.SaveAs(excelFile);
            }
        }

        #region Вспомогательные методы
        private static Department GetOrCreateDepartment(InventoryDataBaseContext db, string name)
        {
            var department = db.Department.FirstOrDefault(d => d.Name == name);
            if (department == null)
            {
                department = new Department { Name = name };
                db.Department.Add(department);
                db.SaveChanges();
            }
            return department;
        }

        private static DeviceType GetOrCreateDeviceType(InventoryDataBaseContext db, string type)
        {
            var deviceType = db.Devicetype.FirstOrDefault(dt => dt.Type == type);
            if (deviceType == null)
            {
                deviceType = new DeviceType { Type = type };
                db.Devicetype.Add(deviceType);
                db.SaveChanges();
            }
            return deviceType;
        }

        private static Wpf_Inventory_.Model.Model GetOrCreateModel(InventoryDataBaseContext db, string modelName)
        {
            var model = db.Model.FirstOrDefault(m => m.Model1 == modelName);
            if (model == null)
            {
                model = new Wpf_Inventory_.Model.Model { Model1 = modelName };
                db.Model.Add(model);
                db.SaveChanges();
            }
            return model;
        }

        private static Office GetOrCreateOffice(InventoryDataBaseContext db, string officeNum)
        {
            var office = db.Office.FirstOrDefault(o => o.Officenum == officeNum);
            if (office == null)
            {
                office = new Office { Officenum = officeNum };
                db.Office.Add(office);
                db.SaveChanges();
            }
            return office;
        }
    }
}
#endregion