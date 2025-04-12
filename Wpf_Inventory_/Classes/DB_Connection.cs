using Wpf_Inventory_.Model;
using System.IO;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;

namespace Wpf_Inventory_.Classes
{
    public class DB_Connection
    {
        /// <summary>
        /// Режим синхронизации.
        /// </summary>
        public enum DatabaseMode
        {
            OfflineFirst,  // локальная база (SQLite) – главный источник
            OnlineFirst    // удалённая база (Postgres) – главный источник
        }

        public static DatabaseMode Mode { get; set; } = DatabaseMode.OnlineFirst;
        public static string CachePath = "cache.sqlite";

        public static InventoryDataBaseContext GetDataBase()
        {
            if (Mode == DatabaseMode.OfflineFirst && File.Exists(CachePath))
            {
                return new InventoryDataBaseContext(
                    new DbContextOptionsBuilder<InventoryDataBaseContext>()
                    .UseSqlite($"Data Source={CachePath}")
                    .Options);
            }

            var postgresContext = new InventoryDataBaseContext();

            if (Mode == DatabaseMode.OnlineFirst)
            {
                try
                {
                    // Проверка соединения
                    postgresContext.Database.OpenConnection();
                    postgresContext.Database.CloseConnection();

                    // Кэшируем в фоне, используя отдельный экземпляр
                    Task.Run(() =>
                    {
                        using var freshContext = new InventoryDataBaseContext();
                        DataCacheService.SaveSnapshot(freshContext);
                    });

                    return postgresContext;
                }
                catch
                {
                    if (File.Exists(CachePath))
                    {
                        return new InventoryDataBaseContext(
                            new DbContextOptionsBuilder<InventoryDataBaseContext>()
                            .UseSqlite($"Data Source={CachePath}")
                            .Options);
                    }

                    throw;
                }
            }

            if (Mode == DatabaseMode.OfflineFirst)
                throw new InvalidOperationException("Нет локального кэша SQLite и соединение с Postgres запрещено в OfflineFirst.");

            return postgresContext;
        }
    }
}