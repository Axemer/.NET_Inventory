using ClosedXML.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using Wpf_Inventory_.Classes;
using Wpf_Inventory_.Model;

namespace Wpf_Inventory_.dbg
{
    internal class ExcelImporter
    {
        public static void ImportDevices(string filePath)
        {
            /// <summary>
            /// Переменная с всем перечнем данных в базе данных.
            /// </summary>
            InventoryDataBaseContext db = DB_Connection.GetDataBase();

            using var document = SpreadsheetDocument.Open(filePath, false);
            var sheet = document.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet>();
            var worksheetPart = (WorksheetPart)document.WorkbookPart.GetPartById(sheet.Id);
            var rows = worksheetPart.Worksheet.Descendants<Row>().Skip(1); // пропустить заголовок

            var sharedStringTable = document.WorkbookPart.SharedStringTablePart?.SharedStringTable;

            string GetCellValue(Cell cell)
            {
                if (cell == null) return null;
                var value = cell.CellValue?.Text;
                if (cell.DataType?.Value == CellValues.SharedString)
                {
                    if (int.TryParse(value, out int idx))
                        return sharedStringTable?.ElementAt(idx)?.InnerText?.Trim();
                }
                return value?.Trim();
            }

            // Кэши для внешних сущностей
            var modelCache = db.Model.ToList().ToDictionary(m => m.Model1?.Trim(), m => m);
            var typeCache = db.Devicetype.ToList().ToDictionary(t => t.Type?.Trim(), t => t);
            var officeCache = db.Office.ToList().ToDictionary(
                o => (o.Block.HasValue ? o.Block.ToString().Trim() : null, o.Department?.Trim(), o.Officenum?.Trim()),
                o => o
            );

            //var existingInventoryNumbers = new HashSet<string>(
            //    db.Device.Select(d => d.Inventorynumber).Where(x => !string.IsNullOrWhiteSpace(x))
            //);
            //var existingIpAddresses = new HashSet<string>(
            //    db.Device.Select(d => d.IpAddress).Where(x => !string.IsNullOrWhiteSpace(x))
            //);

            var devicesToAdd = new List<Device>();

            foreach (var row in rows)
            {
                var cells = row.Elements<Cell>().ToList();

                string ipAddress = GetCellValue(cells.ElementAtOrDefault(0));
                string devicename = GetCellValue(cells.ElementAtOrDefault(1));
                string dateText = GetCellValue(cells.ElementAtOrDefault(2));
                string serialnumber = GetCellValue(cells.ElementAtOrDefault(3));
                string inventorynumber = GetCellValue(cells.ElementAtOrDefault(4));
                string exceptionText = GetCellValue(cells.ElementAtOrDefault(5));
                string note = GetCellValue(cells.ElementAtOrDefault(6));
                string modelName = GetCellValue(cells.ElementAtOrDefault(7))?.Trim('"', ' ');
                string deviceTypeName = GetCellValue(cells.ElementAtOrDefault(8))?.Trim('"', ' ');
                string block = GetCellValue(cells.ElementAtOrDefault(9));
                string department = GetCellValue(cells.ElementAtOrDefault(10));
                string officenum = GetCellValue(cells.ElementAtOrDefault(11));

                if (string.IsNullOrWhiteSpace(devicename)) continue;

                if (!string.IsNullOrWhiteSpace(inventorynumber)) //&& existingInventoryNumbers.Contains(inventorynumber))
                    continue;

                if (!string.IsNullOrWhiteSpace(ipAddress)) //&& existingIpAddresses.Contains(ipAddress))
                    continue;

                modelCache.TryGetValue(modelName, out var model);
                typeCache.TryGetValue(deviceTypeName, out var devicetype);
                officeCache.TryGetValue((block, department, officenum), out var office);

                if (model == null || devicetype == null || office == null)
                    continue;

                DateTime.TryParse(dateText, out var commissioningDate);
                bool exception = exceptionText?.ToLower() == "true";

                var device = new Device
                {
                    Devicename = devicename,
                    IpAddress = ipAddress,
                    Dateofcommissioning = commissioningDate,
                    Serialnumber = serialnumber,
                    Inventorynumber = inventorynumber,
                    Exception = exception,
                    Note = note,
                    ModelId = model.ModelId,
                    DevicetypeId = devicetype.DevicetypeId,
                    OfficeId = office.OfficeId
                };

                devicesToAdd.Add(device);
            }

            db.Device.AddRange(devicesToAdd);
            db.SaveChanges();
        }

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

        /// <summary>
        /// Импортирует устройства из Excel-файла (.xlsx) в базу.
        /// - Игнорирует существующие ID: создаёт новые записи справочников, если их нет.
        /// - Пропускает дубликаты по Inventorynumber и IpAddress.
        /// - Логирует ошибки через Logger.LogError.
        /// </summary>
        public static void Import(string filePath)
        {
            // 1) Читаем все строки Excel в DTO
            var rows = ReadRows(filePath);

            using var db = new InventoryDataBaseContext();

            // 2) Кэшируем существующие записи справочников (только ID)
            var modelMap = db.Model
                .AsNoTracking()
                .ToDictionary(m => m.Model1.Trim(), m => m.ModelId);

            var typeMap = db.Devicetype
                .AsNoTracking()
                .ToDictionary(t => t.Type.Trim(), t => t.DevicetypeId);

            var officeMap = db.Office
                .AsNoTracking()
                .ToList()
                .ToDictionary(
                    o => ((char?)o.Block, o.Department.Trim(), o.Officenum.Trim()),
                    o => o.OfficeId
                );

            // 3) HashSet для отсева дубликатов устройств
            var seenInv = new HashSet<string>(
                db.Device.Select(d => d.Inventorynumber).Where(x => !string.IsNullOrWhiteSpace(x))
            );
            var seenIp = new HashSet<string>(
                db.Device.Select(d => d.IpAddress).Where(x => !string.IsNullOrWhiteSpace(x))
            );

            var devicesToAdd = new List<Device>();

            foreach (var r in rows)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(r.Devicename))
                        continue;

                    // Пропускаем дубли
                    if (!string.IsNullOrWhiteSpace(r.Inventorynumber) && seenInv.Contains(r.Inventorynumber))
                        continue;
                    if (!string.IsNullOrWhiteSpace(r.IpAddress) && seenIp.Contains(r.IpAddress))
                        continue;

                    // Получаем или создаём Model
                    if (!modelMap.TryGetValue(r.ModelName, out var mid))
                    {
                        var m = new Model.Model { Model1 = r.ModelName };
                        db.Model.Add(m);
                        db.SaveChanges();         // получить ID сразу
                        mid = m.ModelId;
                        modelMap[r.ModelName] = mid;
                    }

                    // Получаем или создаём Devicetype
                    if (!typeMap.TryGetValue(r.DeviceTypeName, out var tid))
                    {
                        var t = new Devicetype { Type = r.DeviceTypeName };
                        db.Devicetype.Add(t);
                        db.SaveChanges();
                        tid = t.DevicetypeId;
                        typeMap[r.DeviceTypeName] = tid;
                    }

                    // Получаем или создаём Office
                    var key = (
                        !string.IsNullOrEmpty(r.Block) && char.TryParse(r.Block, out var c) ? (char?)c : null,
                        r.Department?.Trim(),
                        r.OfficeNumber?.Trim()
                    );
                    if (!officeMap.TryGetValue(key, out var oid))
                    {
                        var o = new Office
                        {
                            Block = key.Item1 ?? '\0',
                            Department = key.Item2,
                            Officenum = key.Item3
                        };
                        db.Office.Add(o);
                        db.SaveChanges();
                        oid = o.OfficeId;
                        officeMap[key] = oid;
                    }

                    // Парсим дату и флаг
                    DateTime.TryParse(r.DateText, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt);
                    bool isExc = r.ExceptionText == "true" || r.ExceptionText == "списан";

                    // Создаём Device только по FK
                    devicesToAdd.Add(new Device
                    {
                        Devicename = r.Devicename,
                        IpAddress = r.IpAddress,
                        Dateofcommissioning = dt == default ? null : dt,
                        Serialnumber = r.Serialnumber,
                        Inventorynumber = r.Inventorynumber,
                        Exception = isExc,
                        Note = r.Note,
                        ModelId = mid,
                        DevicetypeId = tid,
                        OfficeId = oid
                    });

                    seenInv.Add(r.Inventorynumber);
                    seenIp.Add(r.IpAddress);
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Row import error: {ex}");
                }
            }

            // 4) Массово вставляем все новые устройства
            if (devicesToAdd.Count > 0)
            {
                db.Device.AddRange(devicesToAdd);
                db.SaveChanges();
            }
        }

        // Чтение Excel в DTO
        private static List<RowDto> ReadRows(string filePath)
        {
            var list = new List<RowDto>();
            using var doc = SpreadsheetDocument.Open(filePath, false);
            var sheet = doc.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet>();
            var wsPart = (WorksheetPart)doc.WorkbookPart.GetPartById(sheet.Id);
            var sst = doc.WorkbookPart.SharedStringTablePart?.SharedStringTable;

            string CellText(Cell c)
            {
                if (c?.CellValue == null) return null;
                var v = c.CellValue.Text;
                if (c.DataType?.Value == CellValues.SharedString && int.TryParse(v, out var i))
                    return sst?.ElementAt(i)?.InnerText;
                return v;
            }

            foreach (var row in wsPart.Worksheet.Descendants<Row>().Skip(1))
            {
                var c = row.Elements<Cell>().ToArray();
                list.Add(new RowDto
                {
                    IpAddress = CellText(c.ElementAtOrDefault(1))?.Trim(),
                    Devicename = CellText(c.ElementAtOrDefault(2))?.Trim(),
                    DateText = CellText(c.ElementAtOrDefault(3))?.Trim(),
                    Serialnumber = CellText(c.ElementAtOrDefault(4))?.Trim(),
                    Inventorynumber = CellText(c.ElementAtOrDefault(5))?.Trim(),
                    ExceptionText = CellText(c.ElementAtOrDefault(6))?.Trim()?.ToLower(),
                    Note = CellText(c.ElementAtOrDefault(7))?.Trim(),
                    ModelName = CellText(c.ElementAtOrDefault(8))?.Trim('"', ' ').Trim(),
                    DeviceTypeName = CellText(c.ElementAtOrDefault(9))?.Trim('"', ' ').Trim(),
                    Block = CellText(c.ElementAtOrDefault(10))?.Trim(),
                    Department = CellText(c.ElementAtOrDefault(11))?.Trim(),
                    OfficeNumber = CellText(c.ElementAtOrDefault(12))?.Trim(),
                });
            }
            return list;
        }

        // Вспомогательный DTO для одной строки Excel
        private class RowDto
        {
            public string IpAddress, Devicename, DateText, Serialnumber,
                          Inventorynumber, ExceptionText, Note,
                          ModelName, DeviceTypeName, Block, Department, OfficeNumber;
        }


        /// <summary>
        /// ////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        private readonly InventoryDataBaseContext _context;

        public ExcelImporter(InventoryDataBaseContext context)
        {
            _context = context;
        }

        public void ImportInventoryData(string filePath)
        {
            // Блок 1: Подготовка и валидация файла
            using var document = SpreadsheetDocument.Open(filePath, false);
            var workbookPart = document.WorkbookPart;
            var worksheet = workbookPart.WorksheetParts.First().Worksheet;
            var sheetData = worksheet.Elements<SheetData>().First();

            // Получаем SharedStringTable для обработки текстовых значений
            var stringTable = workbookPart.SharedStringTablePart.SharedStringTable;

            // Блок 2: Обработка строк данных
            foreach (var row in sheetData.Elements<Row>().Skip(1)) // Пропускаем заголовок
            {
                try
                {
                    var cells = row.Elements<Cell>().ToList();

                    // Блок 2.1: Парсинг значений из ячеек
                    string ipAddress = GetCellValue(cells[1], stringTable);
                    string inventoryNumber = GetCellValue(cells[5], stringTable);

                    // Проверка уникальности IP и инвентарного номера
                    if (_context.Device.Any(d => d.IpAddress == ipAddress))
                    {
                        Logger.LogError($"Дубликат IP: {ipAddress}. Строка {row.RowIndex}");
                        continue;
                    }

                    if (_context.Device.Any(d => d.Inventorynumber == inventoryNumber))
                    {
                        Logger.LogError($"Дубликат инвентарного номера: {inventoryNumber}. Строка {row.RowIndex}");
                        continue;
                    }

                    // Блок 2.2: Обработка офиса
                    char officeBlock = GetCellValue(cells[9], stringTable).Trim()[0]; // Char преобразование
                    var officeDepartment = GetCellValue(cells[10], stringTable);
                    var officeNumber = GetCellValue(cells[11], stringTable).Split('.')[0]; // Убираем .0 из числа

                    var office = _context.Office.FirstOrDefault(o =>
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
                        _context.Office.Add(office);
                        _context.SaveChanges(); // Сохраняем сразу для получения ID
                    }

                    // Блок 2.3: Обработка модели
                    var modelName = GetCellValue(cells[8], stringTable).Replace("\"", ""); // Удаляем кавычки
                    var model = _context.Model.FirstOrDefault(m => m.Model1 == modelName);

                    if (model == null)
                    {
                        model = new Model.Model { Model1 = modelName };
                        _context.Model.Add(model);
                        _context.SaveChanges();
                    }

                    // Блок 2.4: Обработка типа устройства
                    var deviceTypeName = GetCellValue(cells[9], stringTable);
                    var deviceType = _context.Devicetype.FirstOrDefault(dt => dt.Type == deviceTypeName);

                    if (deviceType == null)
                    {
                        deviceType = new Devicetype { Type = deviceTypeName };
                        _context.Devicetype.Add(deviceType);
                        _context.SaveChanges();
                    }

                    // Блок 2.5: Создание устройства
                    var device = new Device
                    {
                        IpAddress = ipAddress,
                        Devicename = GetCellValue(cells[2], stringTable),
                        Dateofcommissioning = DateTime.ParseExact(
                            GetCellValue(cells[3], stringTable),
                            "dd.MM.yyyy HH:mm:ss",
                            CultureInfo.InvariantCulture),
                        Serialnumber = GetCellValue(cells[4], stringTable),
                        Inventorynumber = inventoryNumber,
                        Exception = bool.TryParse(GetCellValue(cells[6], stringTable), out var exception) && exception,
                        Note = GetCellValue(cells[7], stringTable),
                        OfficeId = office.OfficeId,
                        ModelId = model.ModelId,
                        DevicetypeId = deviceType.DevicetypeId
                    };

                    _context.Device.Add(device);
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Ошибка в строке {row.RowIndex}: {ex.Message}");
                }
            }
        }

        // Блок 3: Вспомогательный метод для чтения значений ячеек
        private static string GetCellValue(Cell cell, SharedStringTable stringTable)
        {
            if (cell.DataType?.Value == CellValues.SharedString)
            {
                return stringTable.ElementAt(int.Parse(cell.CellValue.Text)).InnerText;
            }
            return cell.CellValue?.Text ?? string.Empty;
        }

    }
}
