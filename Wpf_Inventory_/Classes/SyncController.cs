using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Wpf_Inventory_.Model;

namespace Wpf_Inventory_.Classes
{
    /// <summary>
    /// Класс для ручного управления синхронизацией данных между локальным кэшем и основной базой данных.
    /// </summary>
    public static class SyncController
    {
        /// <summary>
        /// Управляет автоматической синхронизацией при запуске и подключении к базе.
        /// Если false — никакие операции не происходят автоматически.
        /// </summary>
        public static bool IsAutoSyncEnabled { get; set; } = true;

        /// <summary>
        /// Явно загружает данные из внешней базы (Postgres) и сохраняет их в локальный кэш (SQLite).
        /// Вызывает SaveSnapshot().
        /// </summary>
        public static void ImportFromExternalDatabase()
        {
            using var externalContext = new InventoryDataBaseContext(); // Postgres
            DataCacheService.SaveSnapshot(externalContext);
        }

        /// <summary>
        /// Явно выгружает данные из локального кэша (SQLite) в основную базу (Postgres).
        /// ВАЖНО: текущая реализация может перезаписывать данные в Postgres.
        /// </summary>
        public static void ExportToExternalDatabase()
        {
            if (!File.Exists(DB_Connection.CachePath))
                throw new FileNotFoundException("Локальный кэш SQLite не найден.");

            var cacheOptions = new DbContextOptionsBuilder<InventoryDataBaseContext>()
                .UseSqlite($"Data Source={DB_Connection.CachePath}")
                .Options;

            using var localCacheContext = new InventoryDataBaseContext(cacheOptions);
            using var postgresContext = new InventoryDataBaseContext();

            DataCacheService.SaveSnapshot(localCacheContext);
            postgresContext.SaveChanges();
        }

        /// <summary>
        /// Используется в startup или при вызове GetDataBase().
        /// Если включена автоматическая синхронизация, вызывается кэширование из Postgres.
        /// Если отключена — ничего не происходит.
        /// </summary>
        public static void AutoSyncIfEnabled()
        {
            if (!IsAutoSyncEnabled)
                return;

            Task.Run(() =>
            {
                try
                {
                    using var freshContext = new InventoryDataBaseContext();
                    DataCacheService.SaveSnapshot(freshContext);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Ошибка автосинхронизации: " + ex.Message);
                }
            });
        }
    }
}
