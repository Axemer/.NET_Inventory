using System;

namespace Wpf_Inventory_.Classes
{
    /// <summary>
    /// Класс для логирования ошибок или действий.
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// Метод для логирования ошибок в текстовый файл с датой.
        /// </summary>
        /// <param name="message">Все ошибки в одной строке</param>
        public static void LogError(string message)
        {
            System.IO.File.AppendAllText("error_log.txt", $"\n{DateTime.Now}:\n {message}{Environment.NewLine}");
        }
    }
}
