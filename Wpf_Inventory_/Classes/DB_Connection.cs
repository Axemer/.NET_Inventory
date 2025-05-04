using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Wpf_Inventory_.Model;

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
        public static DatabaseMode Mode { get; set; } = DatabaseMode.OnlineFirst;

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
        private static InventoryDataBaseContext _cachedContext;

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

        /// <summary>
        /// Создает пустую базу данных SQLite.
        /// На слуйчай, если нужно создать кэш и нет доступа к основной базе.
        /// </summary>
        public static void CreateEmptyCache()
        {
            if (File.Exists(CachePath))
                return;

            var options = new DbContextOptionsBuilder<InventoryDataBaseContext>()
                .UseSqlite($"Data Source={CachePath}")
                .Options;

            using var emptyContext = new InventoryDataBaseContext(options);

            // Создаём базу
            emptyContext.Database.EnsureCreated();

            // Пример добавления стартовых значений
            //emptyContext.Devicetype.Add(new Devicetype { Type = "Общий" });
            //emptyContext.SaveChanges();

            //// Установка начальных значений
            var sequenceInitSql = new[]
            {
                "INSERT INTO sqlite_sequence (name, seq) VALUES ('device', 0);",
                "INSERT INTO sqlite_sequence (name, seq) VALUES ('office', 0);",
                "INSERT INTO sqlite_sequence (name, seq) VALUES ('devicetype', 0);",
                "INSERT INTO sqlite_sequence (name, seq) VALUES ('model', 0);",
                "INSERT INTO sqlite_sequence (name, seq) VALUES ('workplace', 0);",
                "INSERT INTO sqlite_sequence (name, seq) VALUES ('deviceparts', 0);"
                // Добавь другие таблицы по аналогии, если нужно
                // Оно вроде в итоге не помогает                        //НАДО ПЕРЕПРОВЕРИТЬ
            };
            foreach (var sql in sequenceInitSql)
            {
                try
                {
                    emptyContext.Database.ExecuteSqlRaw(sql);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Ошибка инициализации sqlite_sequence: {ex.Message}");
                }
            }                                                                                 /// задел на забив данными таблицы но при пустом кеше но оно не очень надо

            Debug.WriteLine($"Пустая база данных SQLite создана по пути: {CachePath}");
        }

        /// <summary>
        /// Сбрасывает кэшированный контекст базы данных из озу.
        /// </summary>
        public static void ResetContext()
        {
            if (_cachedContext != null)
            {
                _cachedContext.Dispose();
                _cachedContext = null;
            }
        }

        /// <summary>
        /// Переключает текущий режим работы базы данных между OfflineFirst и OnlineFirst.
        /// </summary>
        public static void ToggleDatabaseMode()
        {
            if (Mode == DatabaseMode.OnlineFirst)
                Mode = DatabaseMode.OfflineFirst;
            else
                Mode = DatabaseMode.OnlineFirst;

            ResetContext();

            // Выводим текущий режим в консоль на всякий случай
            Debug.WriteLine($"Режим работы базы данных переключён на: {DB_Connection.Mode}");
        }
    }
}