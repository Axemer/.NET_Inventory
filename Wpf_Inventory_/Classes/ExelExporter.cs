using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using OfficeOpenXml;
using System.IO;
using System.Linq;
using System.Windows;
using Wpf_Inventory_.Model;

namespace Wpf_Inventory_.Classes
{
    public class ExcelExporter
    {
        public void ExportTableToExcel()
        {
            var db = DB_Connection.GetDataBase();
            var devices = db.Device
                .Include(d => d.Devicetype)
                .Include(d => d.Model)
                .Include(d => d.Office)
                .ToList();

            if (devices.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SaveFileDialog saveFileDialog = new()
            {
                Filter = "Excel файлы (*.xlsx)|*.xlsx",
                Title = "Сохранить Excel-файл",
                FileName = "InventoryRegistry.xlsx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                SaveToExcelFile(devices, saveFileDialog.FileName);
            }
        }

        private void SaveToExcelFile(System.Collections.Generic.List<Device> devices, string filePath)
        {
#pragma warning disable CS0618
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
#pragma warning restore CS0618

            using ExcelPackage package = new();
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("InventoryRegistry");

            worksheet.Cells[1, 1].Value = "Инвентарный номер";
            worksheet.Cells[1, 2].Value = "Тип устройства";
            worksheet.Cells[1, 3].Value = "Название";
            worksheet.Cells[1, 4].Value = "IP-адрес";
            worksheet.Cells[1, 5].Value = "Модель";
            worksheet.Cells[1, 6].Value = "Серийный номер";
            worksheet.Cells[1, 7].Value = "Дата ввода";
            worksheet.Cells[1, 8].Value = "Офис";
            worksheet.Cells[1, 9].Value = "Отдел";
            worksheet.Cells[1, 10].Value = "Блок";
            worksheet.Cells[1, 11].Value = "Примечание";
            worksheet.Cells[1, 12].Value = "Списано";

            int row = 2;
            foreach (var device in devices)
            {
                worksheet.Cells[row, 1].Value = device.Inventorynumber;
                worksheet.Cells[row, 2].Value = device.Devicetype?.Type;
                worksheet.Cells[row, 3].Value = device.Devicename;
                worksheet.Cells[row, 4].Value = device.IpAddress;
                worksheet.Cells[row, 5].Value = device.Model?.Model1;
                worksheet.Cells[row, 6].Value = device.Serialnumber;
                worksheet.Cells[row, 7].Value = device.Dateofcommissioning?.ToString("dd.MM.yyyy");
                worksheet.Cells[row, 8].Value = device.Office?.Officenum;
                worksheet.Cells[row, 9].Value = device.Office?.Department;
                worksheet.Cells[row, 10].Value = device.Office?.Block.ToString();
                worksheet.Cells[row, 11].Value = device.Note;
                worksheet.Cells[row, 12].Value = device.Exception.HasValue && device.Exception.Value ? "Да" : "Нет";
                row++;
            }

            FileInfo file = new(filePath);
            package.SaveAs(file);
            MessageBox.Show($"Файл успешно сохранен: {filePath}", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}