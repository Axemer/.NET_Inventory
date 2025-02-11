using System;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using OfficeOpenXml;
using Wpf_Inventory_.Classes;

public class ExcelExporter
{
    public void ExportTableToExcel()
    {
        var db = DB_Connection.GetDataBase();
        var inventoryData = db.Device.ToList(); 

        if (inventoryData == null || inventoryData.Count == 0)
        {
            MessageBox.Show("Таблица пуста или не существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        SaveFileDialog saveFileDialog = new SaveFileDialog
        {
            Filter = "Excel файлы (*.xlsx)|*.xlsx",
            Title = "Сохранить Excel-файл",
            FileName = "InventoryRegistry.xlsx"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            SaveToExcelFile(inventoryData, saveFileDialog.FileName);
        }

        if (!inventoryData.Any()) 
        {
            MessageBox.Show("Таблица пуста или не существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
    }

    private void SaveToExcelFile(dynamic data, string filePath)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // без этого щитпоста не работает. прога вроде в инет не стучится для проверки так что плевать, наверное?

        using (ExcelPackage package = new ExcelPackage())
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("InventoryRegistry");

            // Заголовки
            var properties = data[0].GetType().GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = properties[i].Name;
            }

            // Данные
            int row = 2;
            foreach (var item in data)
            {
                for (int col = 0; col < properties.Length; col++)
                {
                    worksheet.Cells[row, col + 1].Value = properties[col].GetValue(item);
                }
                row++;
            }

            FileInfo file = new FileInfo(filePath);
            package.SaveAs(file);
            MessageBox.Show($"Файл успешно сохранен: {filePath}", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
