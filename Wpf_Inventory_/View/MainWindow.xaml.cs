using System;
using System.DirectoryServices.AccountManagement;
using System.Runtime.Versioning;
using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Wpf_Inventory_.Classes;
using Wpf_Inventory_.Model;
using Wpf_Inventory_.View;
using Wpf_Inventory_.View.Controls;
using System.Linq;

namespace Wpf_Inventory_
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

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
            //UpdateStatusIndicator(); // устанавливаем заголовок для режима работы
            UpdateStatus(true);
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
                //StatusTooltip.Text = "Система в сети";
                StatusIndicatorText.Text = "Система в сети";

            }
            else
            {
                StatusIndicator.Fill = Brushes.Red;
                //'StatusTooltip.Text = "Система вне сети";
                StatusIndicatorText.Text = "Система вне сети";
            }
        }



        /// <summary>
        /// Проверяет членство в группе Active Directory.
        /// </summary>
        /// <param name="groupName">Имя группы </param>
        /// <returns></returns>
        [SupportedOSPlatform("windows")] // Add this attribute to indicate the method is Windows-specific
        private static bool IsUserInGroup(string groupName)
        {
            try
            {
                using var context = new PrincipalContext(ContextType.Domain); // This is Windows-specific
                using var user = UserPrincipal.FindByIdentity(context, WindowsIdentity.GetCurrent().Name);
                if (user == null)
                    return false;

                foreach (var group in user.GetGroups())
                {
                    if (group.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }
            catch (PrincipalOperationException ex)
            {
                Logger.LogError($"Ошибка операции с учетной записью: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.LogError($"Ошибка доступа: {ex.Message}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Неизвестная ошибка: {ex.Message}");
            }

            MessageBox.Show("Ошибка проверки группы. Подробности в логах.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            Application.Current.Shutdown(); // Если вход не успешен — закрываем приложение
            return false;
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
                    //ToggleModeMenuItem.Header = "Переключить режим работы в Оффлайн";
                    break;

                case DB_Connection.DatabaseMode.OfflineFirst:
                    StatusIndicator.Fill = new SolidColorBrush(Colors.Purple);
                    StatusIndicatorText.Text = "Оффлайн режим — работа без подключения";
                    //ToggleModeMenuItem.Header = "Переключить режим работы в Онлайн";
                    break;

                default:
                    StatusIndicator.Fill = new SolidColorBrush(Colors.Red);
                    StatusIndicatorText.Text = "ОШИБКА: Нет подключения или нет данных";
                    break;
            }
        }


        /// <summary>
        /// Пулим с базы данных после чего делаем эксель таблицу. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeviceExportButton_Click(object sender, RoutedEventArgs e)
        {
            //ExcelExporter exporter = new();
            //exporter.ExportTableToExcel();
            var visibleDevices = DeviceControl.GetVisibleDevices();
            var exportWindow = new ExportWindow(visibleDevices.Cast<Device>());
            exportWindow.ShowDialog();
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
            switch (DB_Connection.Mode)
            {
                case DB_Connection.DatabaseMode.OnlineFirst:
                    SyncController.ImportFromExternalDatabase();
                    SoftRestart();
                    break;

                case DB_Connection.DatabaseMode.OfflineFirst:
                    MessageBox.Show("Ошибка синхронизации: В режиме оффлайн не возможно сихронизирвоать данные.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);

                    //SyncController.ExportToExternalDatabase();
                    //SoftRestart();  
                    // тк при порытке синхронизации база очищается локально а данные с внешней не сихронизируются.
                    // Итог программа пуста, база не изменена.
                    break;

                default:
                    MessageBox.Show("Ошибка синхронизации: Не выбран режим работы!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
        }

        public void SoftRestart()
        {
            var mainForm = new MainWindow(); // Создать новую форму
            mainForm.Show();
            this.Close(); // Закрыть текущую форму
        }

        private void RebootButton_Click(object sender, RoutedEventArgs e)
        {
            SoftRestart();
        }

        //private void DevicImportButton_Click(object sender, RoutedEventArgs e)
        //{
        //    OpenFileDialog openFileDialog = new()
        //    {
        //        Filter = "Excel файлы (*.xlsx)|*.xlsx",
        //        Title = "Выберите Excel-файл для импорта"
        //    };

        //    if (openFileDialog.ShowDialog() == true)
        //    {
        //        try
        //        {
        //            ExcelImporter.ImportFromExcelFile(openFileDialog.FileName);

        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show($"Ошибка при импорте: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        //        }
        //    }
        //}

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Программа: Система учета данных инвентаря\n" +
                            "Разработчик: Алексей Гринев Ярослоаваич 2025 год\n" +
                            "Версия: 1.0", "О программе",
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
