using System;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Wpf_Inventory_.Classes;
using Wpf_Inventory_.Model;
using Wpf_Inventory_.View;
using Wpf_Inventory_.View.Controls;

namespace Wpf_Inventory_
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Переменная с всем перечнем данных в базе данных.
        /// </summary>
        //public InventoryDataBaseContext _dbo = DB_Connection.GetDataBase();

        /// <summary>
        /// Имя группы Active Directory у которой будет доступ к проложению.
        /// </summary>
        /// private static readonly string _requredGroup = "-Имя_Группы_Системной_администрации_или_типа_того-";


        public MainWindow()
        {
            // LoginCheck(); // ФИЧА ДОДЕЛАНА ВРОДЕ. УБЕРАТЬ ПРИ РЕЛИЗЕ ИЛИ ТЕСТЕ

            //if (IsUserInGroup(_requredGroup) == true)
            //    InitializeComponent();

            InitializeComponent();
            UpdateStatusIndicator(); // устанавливаем заголовок для режима работы
        }

        public void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Метод внедрения проверки на логин.
        /// </summary>
        private static void LoginCheck()
        {
            LoginWindow loginWindow = new();
            if (loginWindow.ShowDialog() != true)
            {
                Application.Current.Shutdown(); // Если вход не успешен — закрываем приложение
            }
        }

        /// <summary>
        /// Позволяет менять цвет индикатора активности
        /// </summary>
        /// <param name="isActive"> true green, false Red </param>
        public void UpdateStatus(bool isActive)
        {
            if (isActive)
            {
                StatusIndicator.Fill = Brushes.Green;
                StatusTooltip.Text = "Система в сети";
            }
            else
            {
                StatusIndicator.Fill = Brushes.Red;
                StatusTooltip.Text = "Система вне сети";
            }
        }

        /// <summary>
        /// Проверяет членство в группе Active Directory.
        /// </summary>
        /// <param name="groupName">Имя группы </param>
        /// <returns></returns>
        //private static bool IsUserInGroup(string groupName)
        //{
        //    try
        //    {
        //        using (var context = new PrincipalContext(ContextType.Domain))
        //        {
        //            using (var user = UserPrincipal.FindByIdentity(context, WindowsIdentity.GetCurrent().Name))
        //            {
        //                if (user == null)
        //                    return false;

        //                foreach (var group in user.GetGroups())
        //                {
        //                    if (group.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase))
        //                        return true;
        //                }
        //            }
        //        }
        //    }
        //    catch (PrincipalOperationException ex)
        //    {
        //        LogError($"Ошибка операции с учетной записью: {ex.Message}");
        //    }
        //    catch (UnauthorizedAccessException ex)
        //    {
        //        LogError($"Ошибка доступа: {ex.Message}");
        //    }
        //    catch (Exception ex)
        //    {
        //        LogError($"Неизвестная ошибка: {ex.Message}");
        //    }

        //    MessageBox.Show("Ошибка проверки группы. Подробности в логах.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        //    Application.Current.Shutdown(); // Если вход не успешен — закрываем приложение
        //    return false;
        //}

        /// <summary>
        /// Реализация логирования ошибки (например, запись в файл или журнал событий)
        /// Пока чисто для примера
        /// </summary>
        private static void LogError(string message)
        {
            System.IO.File.AppendAllText("error_log.txt", $"{DateTime.Now}: {message}{Environment.NewLine}");
        }

        /// <summary>
        /// Обновляет индикатор статуса и текст в зависимости от текущего режима работы.
        /// </summary>
        private void UpdateStatusIndicator()
        {
            if (StatusIndicator == null || StatusIndicatorText == null) return;

            switch (DB_Connection.Mode)
            {
                case DB_Connection.DatabaseMode.OnlineFirst:
                    StatusIndicator.Fill = new SolidColorBrush(Colors.Green);
                    StatusIndicatorText.Text = "Онлайн режим — подключение установлено";
                    break;

                case DB_Connection.DatabaseMode.OfflineFirst:
                    StatusIndicator.Fill = new SolidColorBrush(Colors.Purple);
                    StatusIndicatorText.Text = "Оффлайн режим — работа без подключения";
                    break;

                default:
                    StatusIndicator.Fill = new SolidColorBrush(Colors.Red);
                    StatusIndicatorText.Text = "Нет подключения или нет данных";
                    break;
            }
        }


        /// <summary>
        /// Пулим с базы данных после чего делаем эксель таблицу. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [Obsolete]
        private void DeviceExportButton_Click(object sender, RoutedEventArgs e)
        {
            ExcelExporter exporter = new();
            exporter.ExportTableToExcel();
        }

        /// <summary>
        /// Пинг сервера.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PingServerButton_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = "Data Source=SERVER_NAME;Initial Catalog=DB_NAME;Integrated Security=True;";
            DatabasePinger dbPinger = new(connectionString);
            dbPinger.PingServer();
        }

        /// <summary>
        /// Пинг базы данных.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PingDBButton_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = "Data Source=SERVER_NAME;Initial Catalog=DB_NAME;Integrated Security=True;";
            DatabasePinger dbPinger = new(connectionString);
            dbPinger.PingDatabase();
        }

        private void DeviceSyncButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeviceSaveButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DevicImportButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Программа: Система учета данных инвенторя\n" +
                            "Разработчик: Axemer 2025 год\n" +
                            "Версия: Альфа 0.41", "О программе", 
                            MessageBoxButton.OK, MessageBoxImage.Information);

        }

        private void LoadFromCache_Click(object sender, RoutedEventArgs e)
        {
            DeviceControl d = new();
            d.LoadFromCache();

        }

        private void LoadFromServer_Click(object sender, RoutedEventArgs e)
        {
            DeviceControl d = new();
            d.LoadFromServer();

        }

        private void CreateEmpytyCache_Click(object sender, RoutedEventArgs e)
        {
            DB_Connection.CreateEmptyCache();
        }

        private void ToggleMode_Click(object sender, RoutedEventArgs e)
        {
            DB_Connection.ToggleDatabaseMode(); // Переключить режим базы данных
            UpdateStatusIndicator();            // Обновить статусбар (индикатор и текст)

            DeviceControl d = new();
            d.RefreshContexAndDeviceGrid();     // Обновить таблицу устройств
        }


        /// <summary>
        /// Закрывает окно
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
