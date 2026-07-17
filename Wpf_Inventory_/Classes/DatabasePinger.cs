using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Windows;
using Npgsql;

namespace Wpf_Inventory_.Classes
{
    internal class DatabasePinger
    {
        private readonly string _connectionString;
        private readonly string _host;

        public DatabasePinger(string connectionString)
        {
            _connectionString = connectionString;
            _host = ExtractHost(connectionString);
        }

        private static string ExtractHost(string connectionString)
        {
            var match = Regex.Match(connectionString, @"(?:Server|Data Source|Host)\s*=\s*([^;]+)", RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value.Trim() : "localhost";
        }

        /// <summary>
        /// Проверяет подкоючение к базе данных с временем ответа
        /// </summary>
        public void PingDatabase()
        {
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    stopwatch.Start();
                    connection.Open();
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

                    PingReply reply = ping.Send(_host);

                    stopwatch.Stop();

                    if (reply.Status == IPStatus.Success)
                    {
                        MessageBox.Show($"Сервер {_host} доступен.\nВремя отклика: {reply.RoundtripTime} мс",
                            "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Ошибка пинга {_host}.\nСтатус: {reply.Status}",
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
