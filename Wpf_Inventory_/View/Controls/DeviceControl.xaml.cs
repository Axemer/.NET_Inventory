using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Wpf_Inventory_.Classes;
using Wpf_Inventory_.Model;
using Microsoft.EntityFrameworkCore;

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
        public InventoryDataBaseContext _dbo = DB_Connection.GetDataBase();

        /// <summary>
        /// Представление коллекции для фильтрации данных
        /// </summary>
        private readonly ICollectionView _deviceCollectionView;

        public DeviceControl()
        {
            InitializeComponent();
            DeviceDataGridInit(_dbo);
            _deviceCollectionView = CollectionViewSource.GetDefaultView(DeviceDataGrid.ItemsSource);
        }

        /// <summary>
        ///  Добавляет новое устройство с базовыми значениями
        /// </summary>
        public void AddNewDevice()
        {

            // Создаём новый объект Device
            Device newDevice = new()
            {
                Devicename = "Новое устройство",
                Serialnumber = "0",
                Inventorynumber = "0",
                Model = null,
                Note = "",
                Dateofcommissioning = DateTime.Now,
                DevicetypeId = 1, // Установить позже
                OfficeId = 1, // Установить позже
            };

            // Добавляем в базу данных
            _dbo.Device.Add(newDevice);
            _dbo.SaveChanges(); // Сохраняем в базе, чтобы появился ID тк он присвается базой

            // Открываем окно редактирования нового устройства
            DataViewerWindow dataViewerWindow = new();
            dataViewerWindow.Show();
            dataViewerWindow.ShowData(newDevice, _dbo);
        }

        /// <summary>
        /// Забываем список данным из бд.
        /// </summary>
        /// <param name="InventoryRegDB"> Данные из БД сюда надо </param>
        public void DeviceDataGridInit(InventoryDataBaseContext InventoryRegDB)
        {
            DeviceDataGrid.ItemsSource = InventoryRegDB.Device.Include(d => d.Devicetype)
                                        .ToList();
        }

        /// <summary>
        /// Метод слушатель события сохранения 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDeviceSaved()
        {
            // MessageBox.Show("Событие сохранения сработало!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information); // Для отладки

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

                DataViewerWindow dataViewerWindow = new();
                dataViewerWindow.SaveButtonClicked += OnDeviceSaved;
                dataViewerWindow.Show();
                dataViewerWindow.ShowData(SelctedDevice, _dbo);
            }
        }

        private void DeviceAddButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewDevice();
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Поиск"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string filterText = SearchTextBox.Text.ToLower();
            string selectedCriteria = (SearchCriteriaComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (_deviceCollectionView != null)
            {
                _deviceCollectionView.Filter = item =>
                {
                    if (item is not Device device)
                        return false;

                    // Выглядет больно для чтения но хз стандарт таков теперь
                    return selectedCriteria switch
                    {
                        "Название" => !string.IsNullOrEmpty(device.Devicename) && device.Devicename.Contains(filterText, StringComparison.CurrentCultureIgnoreCase),
                        "Инвентарный номер" => !string.IsNullOrEmpty(device.Inventorynumber) && device.Inventorynumber.Contains(filterText, StringComparison.CurrentCultureIgnoreCase),
                        "IP" => !string.IsNullOrEmpty(device.IpAddress) && device.IpAddress.Contains(filterText, StringComparison.CurrentCultureIgnoreCase),
                        "Дата приема" => device.Dateofcommissioning.ToString().Contains(filterText, StringComparison.CurrentCultureIgnoreCase),
                        _ => true,
                    };
                };
                _deviceCollectionView.Refresh();
            }
        }
    }
}
