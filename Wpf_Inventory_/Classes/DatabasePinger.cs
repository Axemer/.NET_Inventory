using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Wpf_Inventory_.Classes
{
    internal class DatabasePinger
    {
        private readonly string _connectionString;

        public DatabasePinger(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void PingDatabase()
        {
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    stopwatch.Start();
                    connection.Open(); // Пытаемся открыть соединение
                    stopwatch.Stop();

                    MessageBox.Show($"База данных доступна.\nВремя отклика: {stopwatch.ElapsedMilliseconds} мс",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                MessageBox.Show($"Ошибка подключения к базе данных.\nВремя попытки: {stopwatch.ElapsedMilliseconds} мс\nОшибка: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
