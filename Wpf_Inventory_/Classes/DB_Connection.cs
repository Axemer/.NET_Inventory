using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;
using Wpf_Inventory_.Model;
using System.IO;

namespace Wpf_Inventory_.Classes
{
    public class DB_Connection
    {
        /// <summary>
        /// Догадайся
        /// </summary>
        private static InventoryDataBaseContext context;

        /// <summary>
        /// Подключаемся к базе и возвращаем с нее данные
        /// </summary>
        /// <returns></returns>
        public static InventoryDataBaseContext GetDataBase2()
        {
            if (context == null)
                context = new InventoryDataBaseContext();
            return context;
        }

        /// <summary>
        /// Перекилючатель режима Оффлайн/Онлайн
        /// </summary>
        public static bool UseCacheMode { get; set; } = false;

        /// <summary>
        /// Путь к кэшу
        /// </summary>
        private static string CachePath => "Cache.json";

        /// <summary>
        /// Подключаемся к базе и возвращаем с нее контекст для данных
        /// Или выкачиваем данные из кэша и даем их вместо контекста
        /// </summary>
        /// <returns>Строка для обращения к данным приложения</returns>
        public static InventoryDataBaseContext GetDataBase()
        {
            context = new InventoryDataBaseContext();

            if (UseCacheMode == true)
            {
                if (File.Exists(CachePath))
                {
                    var snapshot = LoadSnapshot();
                    LoadToContextFromCache(context, snapshot);
                }
            }

            return context;
        }

        /// <summary>
        /// Сохраняем данные из контекста в кэш
        /// </summary>
        /// <param name="context"></param>
        public static void SaveSnapshotFromContext(InventoryDataBaseContext context)
        {
            var snapshot = new DatabaseSnapshot
            {
                Devices = context.Device.ToList(),
                Devicetypes = context.Devicetype.ToList(),
                Deviceparts = context.Deviceparts.ToList(),
                Offices = context.Office.ToList(),
                DeviceWorkplaces = context.DeviceWorkplace.ToList(),
                DevicepartsDevices = context.DevicepartsDevice.ToList(),
                Models = context.Model.ToList(),
                Workplaces = context.Workplace.ToList()
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };

            var json = JsonSerializer.Serialize(snapshot, options);
            File.WriteAllText(CachePath, json);
        }

        /// <summary>
        /// Загружаем данные из кэша
        /// </summary>
        /// <returns></returns>
        internal static DatabaseSnapshot LoadSnapshot()
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };

            var json = File.ReadAllText(CachePath);
            return JsonSerializer.Deserialize<DatabaseSnapshot>(json, options)!;
        }

        /// <summary>
        /// Переносим данные из кэша в контекст
        /// </summary>
        /// <param name="context">Цеоеврй контекст</param>
        /// <param name="snapshot">Целевой кэш</param>
        internal static void LoadToContextFromCache(InventoryDataBaseContext context, DatabaseSnapshot snapshot)
        {
            context.ChangeTracker.Clear();

            context.Device.Local.Clear();
            context.Devicetype.Local.Clear();
            context.Deviceparts.Local.Clear();
            context.Office.Local.Clear();
            context.DeviceWorkplace.Local.Clear();
            context.DevicepartsDevice.Local.Clear();
            context.Model.Local.Clear();
            context.Workplace.Local.Clear();

            context.Device.AddRange(snapshot.Devices);
            context.Devicetype.AddRange(snapshot.Devicetypes);
            context.Deviceparts.AddRange(snapshot.Deviceparts);
            context.Office.AddRange(snapshot.Offices);
            context.DeviceWorkplace.AddRange(snapshot.DeviceWorkplaces);
            context.DevicepartsDevice.AddRange(snapshot.DevicepartsDevices);
            context.Model.AddRange(snapshot.Models);
            context.Workplace.AddRange(snapshot.Workplaces);
        }
    }
}