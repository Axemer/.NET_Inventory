using Wpf_Inventory_.Model;
using System.IO;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using System.Diagnostics;

namespace Wpf_Inventory_.Classes
{
    public class DB_Connection
    {
        public enum DatabaseMode
        {
            OfflineFirst, // Работает только с локальным кэшем SQLite и уже после вносит изменения в Postgres.
            OnlineFirst   // Работает только с Postgres без кеша.
        }

        /// <summary>
        /// Переключатель режима работы с базой данных.
        /// </summary>
        public static DatabaseMode Mode { get; set; } = DatabaseMode.OfflineFirst;

        /// <summary>
        /// Путь к локальному кэшу SQLite.
        /// По умолчанию в корне программы.
        /// </summary>
        public static readonly string CachePath = "cache.sqlite";

        /// <summary>
        /// Путь к резервному "Аварийному" локальному кэшу SQLite.
        /// </summary>
        private static readonly string BackupCachePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\BackUpCache.sqlite");

        /// <summary>
        /// Экземпляр кэща InventoryDataBaseContext.
        /// </summary>
        private static InventoryDataBaseContext? _cachedContext;

        /// <summary>
        /// Получает экземпляр InventoryDataBaseContext и 
        /// генерирует локальный кеш на его основе.
        /// </summary>
        /// <returns>Дает экземпляр InventoryDataBaseContext в виде контекста для EF</returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <summary>
        /// Возвращает экземпляр контекста базы данных в зависимости от текущего режима.
        /// </summary>
        public static InventoryDataBaseContext GetDataBase()
        {

            Debug.WriteLine("CachePath exists: " + File.Exists(CachePath));
            Debug.WriteLine("BackupCachePath exists: " + File.Exists(BackupCachePath));
            Debug.WriteLine("Backup path: " + BackupCachePath);

            if (_cachedContext != null)
                return _cachedContext;

            // OFFLINE MODE
            if (Mode == DatabaseMode.OfflineFirst)
            {
                if (File.Exists(CachePath))
                {
                    _cachedContext = new InventoryDataBaseContext(
                        new DbContextOptionsBuilder<InventoryDataBaseContext>()
                        .UseSqlite($"Data Source={CachePath}")
                        .Options);
                    return _cachedContext;
                }

                var BackupCachePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\BackUpCache.sqlite");
                if (File.Exists(BackupCachePath))
                {
                    _cachedContext = new InventoryDataBaseContext(
                        new DbContextOptionsBuilder<InventoryDataBaseContext>()
                        .UseSqlite($"Data Source={BackupCachePath}")
                        .Options);
                    return _cachedContext;
                }

                throw new InvalidOperationException("Нет доступа ни к основной, ни к резервной базе данных.");
            }

            // ONLINE MODE
            var postgresContext = new InventoryDataBaseContext();
            try
            {
                postgresContext.Database.OpenConnection();
                postgresContext.Database.CloseConnection();

                // Запускаем фоновое кэширование чтоб прога не встала думать
                Task.Run(() =>
                {
                    using var freshContext = new InventoryDataBaseContext();
                    DataCacheService.SaveSnapshot(freshContext);
                });

                _cachedContext = postgresContext;
                return _cachedContext;
            }
            catch
            {
                // Используем локальный кэш, если он есть
                if (File.Exists(CachePath))
                {
                    _cachedContext = new InventoryDataBaseContext(
                        new DbContextOptionsBuilder<InventoryDataBaseContext>()
                        .UseSqlite($"Data Source={CachePath}")
                        .Options);
                    return _cachedContext;
                }

                var BackupCachePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\BackUpCache.sqlite");
                if (File.Exists(BackupCachePath))
                {
                    _cachedContext = new InventoryDataBaseContext(
                        new DbContextOptionsBuilder<InventoryDataBaseContext>()
                        .UseSqlite($"Data Source={BackupCachePath}")
                        .Options);
                    return _cachedContext;
                }

                throw;
            }
        }
    }

}