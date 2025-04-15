using Wpf_Inventory_.Model;
using System.IO;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;

namespace Wpf_Inventory_.Classes
{
    public static class DataCacheService
    {
        public static string CachePath = "cache.sqlite";

        public static void SaveSnapshot(InventoryDataBaseContext source)
        {
            Console.WriteLine(" SaveSnapshot() начат."); /// debug sh*t

            var options = new DbContextOptionsBuilder<InventoryDataBaseContext>()
                .UseSqlite($"Data Source={CachePath}")
                .Options;

            using var sqliteContext = new InventoryDataBaseContext(options);
            sqliteContext.Database.EnsureDeleted();
            sqliteContext.Database.EnsureCreated();

            CopyData(source, sqliteContext);
            sqliteContext.SaveChanges();
        }

        public static void LoadSnapshotToContext(InventoryDataBaseContext target)
        {
            if (!File.Exists(CachePath)) return;

            var options = new DbContextOptionsBuilder<InventoryDataBaseContext>()
                .UseSqlite($"Data Source={CachePath}")
                .Options;

            using var sqliteContext = new InventoryDataBaseContext(options);
            CopyData(sqliteContext, target);
        }

        private static void CopyData(InventoryDataBaseContext source, InventoryDataBaseContext target)
        {
            // Очистка таблиц (очерёдность важна для FK!)
            target.DevicepartsDevice.RemoveRange(target.DevicepartsDevice);
            target.DeviceWorkplace.RemoveRange(target.DeviceWorkplace);
            target.Device.RemoveRange(target.Device);
            target.Model.RemoveRange(target.Model);
            target.Office.RemoveRange(target.Office);
            target.Workplace.RemoveRange(target.Workplace);
            target.Devicetype.RemoveRange(target.Devicetype);
            target.Deviceparts.RemoveRange(target.Deviceparts);

            target.SaveChanges(); // Фиксация удаления перед вставкой

            //// dubug sh*t ///////////////////////////////////////////////////////////////////
            var duplicates = source.Devicetype.AsNoTracking().ToList() // Переводим в память
                            .GroupBy(d => d.DevicetypeId).Where(g => g.Count() > 1).ToList();
            if (duplicates.Any())
            {
                Console.WriteLine("Найдены дубликаты DevicetypeId:");
                foreach (var group in duplicates)
                    Console.WriteLine($"  Id = {group.Key}, Count = {group.Count()}");

                throw new Exception("Найдены дубликаты в Devicetype!");
            }
            //// dubug sh*t ///////////////////////////////////////////////////////////////////

            // Загрузка данных
            target.Devicetype.AddRange(source.Devicetype.AsNoTracking());
            target.Model.AddRange(source.Model.AsNoTracking());
            target.Office.AddRange(source.Office.AsNoTracking());
            target.Workplace.AddRange(source.Workplace.AsNoTracking());
            target.Deviceparts.AddRange(source.Deviceparts.AsNoTracking());
            target.Device.AddRange(source.Device.AsNoTracking());
            target.DevicepartsDevice.AddRange(source.DevicepartsDevice.AsNoTracking());
            target.DeviceWorkplace.AddRange(source.DeviceWorkplace.AsNoTracking());

            var allDevicetypes = source.Devicetype.AsNoTracking().ToList();
            Console.WriteLine($"🔍 Загружено Devicetype: {allDevicetypes.Count}");
            foreach (var dev in allDevicetypes)
                Console.WriteLine($"Devicetype ID: {dev.DevicetypeId}");


        }
    }
}
