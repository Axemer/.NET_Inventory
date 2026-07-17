using OfficeOpenXml;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using Wpf_Inventory_.Model;
using Microsoft.EntityFrameworkCore;

namespace Wpf_Inventory_.Classes
{
    internal class ExcelCommunication
    {
        public static void ImportFromExcel(string filePath, InventoryDataBaseContext dbContext)
        {
            FileInfo fileInfo = new FileInfo(filePath);

#pragma warning disable CS0618
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
#pragma warning restore CS0618

            using (var package = new ExcelPackage(fileInfo))
            {
                var worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        var device = new Device
                        {
                            Devicename = worksheet.Cells[row, 1].Text,
                            IpAddress = worksheet.Cells[row, 2].Text,
                            Serialnumber = worksheet.Cells[row, 3].Text,
                            Inventorynumber = worksheet.Cells[row, 4].Text,
                            Note = worksheet.Cells[row, 5].Text,
                            Dateofcommissioning = DateTime.TryParse(worksheet.Cells[row, 6].Text, out var date) ? date : null,
                            Exception = worksheet.Cells[row, 7].Text.ToLower() == "да"
                        };

                        string deviceTypeName = worksheet.Cells[row, 9].Text;
                        if (!string.IsNullOrWhiteSpace(deviceTypeName))
                        {
                            var devType = dbContext.Devicetype.FirstOrDefault(dt => dt.Type == deviceTypeName);
                            if (devType == null)
                            {
                                devType = new Devicetype { Type = deviceTypeName };
                                dbContext.Devicetype.Add(devType);
                                dbContext.SaveChanges();
                            }
                            device.DevicetypeId = devType.DevicetypeId;
                        }

                        string modelName = worksheet.Cells[row, 10].Text;
                        if (!string.IsNullOrWhiteSpace(modelName))
                        {
                            var model = dbContext.Model.FirstOrDefault(m => m.Model1 == modelName);
                            if (model == null)
                            {
                                model = new Model.Model { Model1 = modelName };
                                dbContext.Model.Add(model);
                                dbContext.SaveChanges();
                            }
                            device.ModelId = model.ModelId;
                        }

                        string officeNum = worksheet.Cells[row, 11].Text;
                        if (!string.IsNullOrWhiteSpace(officeNum))
                        {
                            var office = dbContext.Office.FirstOrDefault(o => o.Officenum == officeNum);
                            if (office == null)
                            {
                                office = new Office { Officenum = officeNum };
                                dbContext.Office.Add(office);
                                dbContext.SaveChanges();
                            }
                            device.OfficeId = office.OfficeId;
                        }

                        if (!dbContext.Device.Any(d => d.Inventorynumber == device.Inventorynumber))
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

        public static void ExportToExcel(string filePath, InventoryDataBaseContext dbContext)
        {
#pragma warning disable CS0618
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
#pragma warning restore CS0618

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Devices");

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

                var devices = dbContext.Device
                    .Include(d => d.Devicetype)
                    .Include(d => d.Model)
                    .Include(d => d.Office)
                    .ToList();

                for (int i = 0; i < devices.Count; i++)
                {
                    var device = devices[i];
                    worksheet.Cells[i + 2, 1].Value = device.Devicename;
                    worksheet.Cells[i + 2, 2].Value = device.IpAddress;
                    worksheet.Cells[i + 2, 3].Value = device.Serialnumber;
                    worksheet.Cells[i + 2, 4].Value = device.Inventorynumber;
                    worksheet.Cells[i + 2, 5].Value = device.Note;
                    worksheet.Cells[i + 2, 6].Value = device.Dateofcommissioning?.ToString("dd.MM.yyyy");
                    worksheet.Cells[i + 2, 7].Value = device.Exception.HasValue && device.Exception.Value ? "Да" : "Нет";
                    worksheet.Cells[i + 2, 8].Value = device.Office?.Department;
                    worksheet.Cells[i + 2, 9].Value = device.Devicetype?.Type;
                    worksheet.Cells[i + 2, 10].Value = device.Model?.Model1;
                    worksheet.Cells[i + 2, 11].Value = device.Office?.Officenum;
                }

                FileInfo excelFile = new FileInfo(filePath);
                package.SaveAs(excelFile);
            }
        }
    }
}