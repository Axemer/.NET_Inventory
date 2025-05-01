using ClosedXML.Excel;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using Wpf_Inventory_.Classes;

namespace Wpf_Inventory_.dbg
{
    public class ExcelExporter
    {
        /// <summary>
        /// Берет данные из базы данных и экспортирует их в эксель файл.
        /// </summary>
        public void ExportTableToExcel()
        {
            var db = DB_Connection.GetDataBase();
            var devices = db.Device.ToList(); // Используйте правильное имя DbSet

            if (devices == null || devices.Count == 0)
            {
                MessageBox.Show("Таблица пуста или не существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Создаем "вычищенный" список с нужными полями
            var exportData = devices.Select(device => new
            {
                device.DeviceId,
                device.IpAddress,
                device.Devicename,
                device.Dateofcommissioning,
                device.Serialnumber,
                device.Inventorynumber,
                device.Exception,
                device.Note,

                Model = device.Model?.Model1,
                DeviceType = device.Devicetype?.Type,
                OfficeBlock = device.Office?.Block,
                OfficeDepartment = device.Office?.Department,
                OfficeNumber = device.Office?.Officenum
            }).ToList();

            SaveFileDialog saveFileDialog = new()
            {
                Filter = "Excel файлы (*.xlsx)|*.xlsx",
                Title = "Сохранить Excel-файл",
                FileName = "InventoryRegistry.xlsx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                SaveToExcelFile(exportData, saveFileDialog.FileName);
            }
        }

        /// <summary>
        /// Дает названия типам данных которые уже можно записать в эксель.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsSimpleType(Type type)
        {
            return type.IsPrimitive
          || type.IsEnum
          || type == typeof(string)
          || type == typeof(decimal)
          || type == typeof(DateTime)
          || type == typeof(Guid)
          || type == typeof(bool);
        }

        /// <summary>
        /// Сохраняет данные в эксель файл.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="filePath"></param>
        private static void SaveToExcelFile(dynamic data, string filePath)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("InventoryRegistry");

            var properties = ((object)data[0]).GetType().GetProperties();

            // Заголовки
            for (int i = 0; i < properties.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = properties[i].Name;
            }

            // Данные
            int row = 2;
            foreach (var item in data)
            {
                for (int col = 0; col < properties.Length; col++)
                {
                    var value = properties[col].GetValue(item);
                    worksheet.Cell(row, col + 1).Value = value?.ToString() ?? string.Empty;
                }
                row++;
            }

            worksheet.Columns().AdjustToContents(); // Автоширина
            workbook.SaveAs(filePath);
            MessageBox.Show($"Файл успешно сохранен: {filePath}", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}