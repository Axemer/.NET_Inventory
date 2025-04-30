using ClosedXML.Excel;
using System;
using System.Linq;
using System.Windows;
using Wpf_Inventory_.Model;

namespace Wpf_Inventory_.Classes
{
    internal class ExcelImporter
    {
        /// <summary>
        /// МЕГА метод для импорта устройств из Excel файла.
        /// </summary>
        /// <param name="filePath"></param>
        public static void ImportFromExcelFile(string filePath)
        {
            var context = new InventoryDataBaseContext();
            var importedCount = 0;

            using var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheets.First();
            var rows = worksheet.RowsUsed().Skip(1); // Пропускаем заголовки

            foreach (var row in rows)
            {
                try
                {
                    // Получаем значения из ячеек
                    var ipAddress = row.Cell(2).GetString(); // Пока не используем
                    var devicename = row.Cell(3).GetString();

                    // Безопасный парсинг даты
                    DateTime? commissioningDate = null;
                    if (DateTime.TryParse(row.Cell(4).GetString(), out var parsedDate))
                        commissioningDate = parsedDate;

                    var serialnumber = row.Cell(5).GetString();
                    var inventorynumber = row.Cell(6).GetString()?.Trim();

                    // Пропускаем, если дубликат по инвентарному номеру
                    //if (string.IsNullOrWhiteSpace(inventorynumber) || context.Device.Any(d => d.Inventorynumber == inventorynumber))
                    //continue;

                    // === Пропуск, если IP-адрес уже существует ===
                    // if (!string.IsNullOrWhiteSpace(ipAddress) && db.Device.Any(d => d.IpAddress == ipAddress))
                    //     continue;

                    // Обработка исключения
                    var exceptionString = row.Cell(7).GetString()?.ToLower();
                    bool? isException = exceptionString == "true" || exceptionString == "списан" ? true : false;

                    var note = row.Cell(8).GetString();
                    var modelName = row.Cell(9).GetString()?.Trim('"');
                    var deviceTypeName = row.Cell(10).GetString();
                    var officeBlockString = row.Cell(11).GetString()?.Trim();
                    var officeDepartment = row.Cell(12).GetString();
                    var officeNumber = row.Cell(13).GetString();

                    // === Найти или создать связанные сущности ===

                    // Модель
                    var model = context.Model.FirstOrDefault(m => m.Model1 == modelName);
                    if (model == null)
                    {
                        model = new Model.Model { Model1 = modelName };
                        context.Model.Add(model);
                        context.SaveChanges();
                    }

                    // Тип устройства
                    var devicetype = context.Devicetype.FirstOrDefault(t => t.Type == deviceTypeName);
                    if (devicetype == null)
                    {
                        devicetype = new Devicetype { Type = deviceTypeName };
                        context.Devicetype.Add(devicetype);
                        context.SaveChanges();
                    }

                    // Офис (block - char!)
                    char? officeBlock = null;
                    if (!string.IsNullOrWhiteSpace(officeBlockString) && char.TryParse(officeBlockString, out var parsedBlock))
                        officeBlock = parsedBlock;

                    var office = context.Office.FirstOrDefault(o =>
                        o.Block == officeBlock &&
                        o.Department == officeDepartment &&
                        o.Officenum == officeNumber);

                    if (office == null)
                    {
                        office = new Office
                        {
                            Block = officeBlock,
                            Department = officeDepartment,
                            Officenum = officeNumber
                        };
                        context.Office.Add(office);
                        context.SaveChanges();
                    }

                    // === Добавление нового устройства ===
                    var device = new Device
                    {
                        IpAddress = null, // пока не используем ipAddress
                        Devicename = devicename,
                        Dateofcommissioning = commissioningDate,
                        Serialnumber = serialnumber,
                        Inventorynumber = inventorynumber,
                        Exception = isException,
                        Note = note,
                        ModelId = model.ModelId,
                        DevicetypeId = devicetype.DevicetypeId,
                        OfficeId = office.OfficeId
                    };

                    context.Device.Add(device);
                    context.SaveChanges();
                    importedCount++;
                }
                catch (Exception ex)
                {
                    // Вывод ошибок с содержимым строки
                    MessageBox.Show(
                        $"Ошибка в строке:\n{string.Join(" | ", row.Cells().Select(c => c.GetString()))}\n\nИсключение:\n{ex}",
                        "Ошибка импорта",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    
                    Logger.LogError($"Ошибка в строке:\n{string.Join(" | ", row.Cells().Select(c => c.GetString()))}\n\nИсключение:\n{ex}");
                }
            }

            MessageBox.Show($"Успешно импортировано устройств: {importedCount}", "Импорт завершён", MessageBoxButton.OK, MessageBoxImage.Information);
        }

    }
}
