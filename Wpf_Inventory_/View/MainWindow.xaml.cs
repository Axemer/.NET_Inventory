using System;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Input;
using System.Windows.Media;
using Wpf_Inventory_.Classes;
using Wpf_Inventory_.dbo;
using Wpf_Inventory_.View;

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
        public InventoryRegistryDataBaseEntities3 _dbo = DB_Connection.GetDataBase();

        /// <summary>
        /// Имя группы Active Directory у которой будет доступ к проложению.
        /// </summary>
        private static readonly string _requredGroup = "-Имя_Группы_Системной_администрации_или_типа_того-";


        public MainWindow()
        {
            // LoginCheck(); // ФИЧА ДОДЕЛАНА ВРОДЕ. УБЕРАТЬ ПРИ РЕЛИЗЕ ИЛИ ТЕСТЕ
            
            //if (IsUserInGroup(_requredGroup) == true)
            //    InitializeComponent();

            InitializeComponent();
     
        }

        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DeviceDataGridInit(_dbo);
        }

        /// <summary>
        /// Метод внедрения проверки на логин.
        /// </summary>
        private void LoginCheck()
        {
            LoginWindow loginWindow = new LoginWindow();
            if (loginWindow.ShowDialog() != true)
            {
                Application.Current.Shutdown(); // Если вход не успешен — закрываем приложение
            }
        }

        /// <summary>
        ///  Добавляет новое устройство с базовыми значениями
        /// </summary>
        public void AddNewDevice()
        {
            // Создаём новый объект Device
            Device newDevice = new Device
            {
                DeviceName = "Новое устройство",
                SerialNumber = "0",
                InventoryNumber = "0",
                Model = null,
                Note = "",
                DateOfCommissioning = DateTime.Now,
                DeviceType_ID = 1, // Установить позже
                Office_ID = 1, // Установить позже
                Department_ID = 1 // Установить позже
            };

            // Добавляем в базу данных
            _dbo.Device.Add(newDevice);
            _dbo.SaveChanges(); // Сохраняем в базе, чтобы появился ID

            // Открываем окно редактирования нового устройства
            DataViewerWindow dataViewerWindow = new DataViewerWindow();
            dataViewerWindow.Show();
            dataViewerWindow.ShowData(newDevice, _dbo);
        }

        /// <summary>
        /// Забываем список данным из бд.
        /// </summary>
        /// <param name="InventoryRegDB"> Данные из БД сюда надо </param>
        public void DeviceDataGridInit(InventoryRegistryDataBaseEntities3 InventoryRegDB)
        {
            //DeviceDataGrid.Items.Clear();
            DeviceDataGrid.ItemsSource = InventoryRegDB.Device.ToList();
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
        private static bool IsUserInGroup(string groupName)
        {
            try
            {
                using (var context = new PrincipalContext(ContextType.Domain))
                {
                    using (var user = UserPrincipal.FindByIdentity(context, WindowsIdentity.GetCurrent().Name))
                    {
                        if (user == null)
                            return false;

                        foreach (var group in user.GetGroups())
                        {
                            if (group.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase))
                                return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка проверки группы: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown(); // Если вход не успешен — закрываем приложение
            }
            return false;
        }

        /// <summary>
        /// Метод слушатель события сохранения 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDeviceSaved()
        {
            MessageBox.Show("Событие сохранения сработало!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);

            DeviceDataGridInit(_dbo);
        }

        private void DeviceDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        /// <summary>
        /// Отрывает окно с подробностями о выбраном элементе БД.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeviceDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var SelctedDevice = this.DeviceDataGrid.SelectedItem;

            if (SelctedDevice != null)
            {
                
                DataViewerWindow dataViewerWindow = new DataViewerWindow();
                dataViewerWindow.SaveButtonClicked += OnDeviceSaved;
                dataViewerWindow.Show();
                dataViewerWindow.ShowData(SelctedDevice, _dbo);
                
            }

        }

        /// <summary>
        /// Пулим с базы данных после чего делаем эксель таблицу. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeviceExportButton_Click(object sender, RoutedEventArgs e)
        {
            ExcelExporter exporter = new ExcelExporter();
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
            DatabasePinger dbPinger = new DatabasePinger(connectionString);
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
            DatabasePinger dbPinger = new DatabasePinger(connectionString);
            dbPinger.PingDatabase();
        }

        private void DeviceAddButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewDevice();
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

        /// <summary>
        /// Закрывает окно
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }


    }
}
