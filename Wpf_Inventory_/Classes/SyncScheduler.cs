using System;
using System.Timers;

namespace Wpf_Inventory_.Classes
{
    internal class SyncScheduler
    {
        private static Timer syncTimer;

        public static void Start()
        {
            syncTimer = new Timer(300000); // Запуск каждые 5 минут
            syncTimer.Elapsed += (sender, e) =>
            {
                try
                {
                    SyncService.SyncWithRemote();
                    Console.WriteLine("Синхронизация выполнена!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка синхронизации: {ex.Message}");
                }
            };
            syncTimer.Start();
        }
    }
}
