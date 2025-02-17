using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net.NetworkInformation;
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

        /// <summary>
        /// Проверяет подкоючение к базе данных с временем ответа
        /// </summary>
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

        /// <summary>
        /// Проверяет подкоючение к серверу с временем ответа
        /// </summary>
        public void PingServer()
        {
            try
            {
                using (Ping ping = new Ping())
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    PingReply reply = ping.Send(_connectionString);

                    stopwatch.Stop();

                    if (reply.Status == IPStatus.Success)
                    {
                        MessageBox.Show($"Сервер доступен.\nВремя отклика: {reply.RoundtripTime} мс",
                            "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Ошибка пинга.\nСтатус: {reply.Status}",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении пинга.\nОшибка: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
