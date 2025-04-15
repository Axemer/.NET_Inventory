using Wpf_Inventory_.Model;
using System.IO;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;

namespace Wpf_Inventory_.Classes
{
    public class DB_Connection
    {
        public enum DatabaseMode
        {
            OfflineFirst,
            OnlineFirst
        }

        public static DatabaseMode Mode { get; set; } = DatabaseMode.OnlineFirst;
        public static string CachePath = "cache.sqlite";

        private static InventoryDataBaseContext? _cachedContext;

        public static InventoryDataBaseContext GetDataBase()
        {
            if (_cachedContext != null)
                return _cachedContext;

            if (Mode == DatabaseMode.OfflineFirst && File.Exists(CachePath))
            {
                _cachedContext = new InventoryDataBaseContext(
                    new DbContextOptionsBuilder<InventoryDataBaseContext>()
                    .UseSqlite($"Data Source={CachePath}")
                    .Options);
                return _cachedContext;
            }

            var postgresContext = new InventoryDataBaseContext();

            if (Mode == DatabaseMode.OnlineFirst)
            {
                try
                {
                    postgresContext.Database.OpenConnection();
                    postgresContext.Database.CloseConnection();

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
                    if (File.Exists(CachePath))
                    {
                        _cachedContext = new InventoryDataBaseContext(
                            new DbContextOptionsBuilder<InventoryDataBaseContext>()
                            .UseSqlite($"Data Source={CachePath}")
                            .Options);
                        return _cachedContext;
                    }

                    throw;
                }
            }

            if (Mode == DatabaseMode.OfflineFirst)
                throw new InvalidOperationException("Нет локального кэша SQLite и соединение с Postgres запрещено в OfflineFirst.");

            _cachedContext = postgresContext;
            return _cachedContext;
        }
    }

}