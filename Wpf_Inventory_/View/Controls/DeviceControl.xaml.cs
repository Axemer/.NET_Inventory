using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf_Inventory_.Classes;
using Wpf_Inventory_.dbo;

namespace Wpf_Inventory_.View.Controls
{
    /// <summary>
    /// Логика взаимодействия для DeviceControl.xaml
    /// </summary>
    public partial class DeviceControl : UserControl
    {
        /// <summary>
        /// Переменная с всем перечнем данных в базе данных.
        /// </summary>
        public InventoryRegistryDataBaseEntities3 _dbo = DB_Connection.GetDataBase();

        public DeviceControl()
        {
            InitializeComponent();
            
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
            _dbo.SaveChanges(); // Сохраняем в базе, чтобы появился ID тк он присвается базой

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

        private void DeviceAddButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewDevice();
        }
    }
}
