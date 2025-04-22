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

        //public InventoryDataBaseContext _dbo =  LocalDatabase.Get

        /// <summary>
        /// Представление коллекции для фильтрации данных
        /// </summary>
        private readonly ICollectionView _deviceCollectionView;

        public DeviceControl()
        {
            InitializeComponent();
            //DeviceDataGridInit(_dbo);



            if (_dbo.Database.CanConnect())
            {
                DB_Connection.Mode = DB_Connection.DatabaseMode.OfflineFirst;
                DeviceDataGridInit(_dbo);
            }

            _deviceCollectionView = CollectionViewSource.GetDefaultView(DeviceDataGrid.ItemsSource);
        }

        public void LoadFromCache()
        {
            //DB_Connection.UseCacheMode = true;
            _dbo = DB_Connection.GetDataBase();
            DeviceDataGrid.ItemsSource = _dbo.Device.Local.ToObservableCollection();
            //DeviceDataGridInit(_dbo);
            //_deviceCollectionView = CollectionViewSource.GetDefaultView(DeviceDataGrid.ItemsSource);
        }

        public void LoadFromServer()
        {
            //DB_Connection.UseCacheMode = false;
            //_dbo = DB_Connection.GetDataBase();
            DeviceDataGridInit(_dbo);
        }

        /// <summary>
        ///  Добавляет новое устройство с базовыми значениями
        /// </summary>
        public void AddNewDevice()
        {
            if (!_dbo.Devicetype.Any())
                _dbo.Devicetype.Add(new Devicetype { DevicetypeId = 1, Type = "Общий" });

            if (!_dbo.Office.Any())
                _dbo.Office.Add(new Office { OfficeId = 1, Officenum = "101", Department = "Отдел A" });

            _dbo.SaveChanges();

            // Создаём новое устройство
            Device newDevice = new()
            {
                Devicename = "Новое устройство",
                Serialnumber = "0",
                Inventorynumber = "0",
                Model = null,
                Note = "",
                Dateofcommissioning = DateTime.Now,
                DevicetypeId = 1,
                OfficeId = 1,
            };

            _dbo.Device.Add(newDevice);
            _dbo.SaveChanges(); 

            DataViewerWindow dataViewerWindow = new();
            dataViewerWindow.SaveButtonClicked += OnDeviceSaved;
            dataViewerWindow.Show();
            dataViewerWindow.ShowData(newDevice, _dbo);
        }

        public ICollectionView Get_deviceCollectionView()
        {
            return _deviceCollectionView;
        }

        /// <summary>
        /// Забиваем список данным из бд.
        /// </summary>
        /// <param name="InventoryRegDB"> Данные из БД сюда надо </param>
        public void DeviceDataGridInit(InventoryDataBaseContext InventoryRegDB)
        {
            //DeviceDataGrid.ItemsSource = InventoryRegDB.Device.Include(d => d.Devicetype).ToList();

            DeviceDataGrid.ItemsSource = InventoryRegDB.Device
                                         .Include(d => d.Devicetype)
                                         .AsNoTracking()
                                         .ToList();

            //_deviceCollectionView = CollectionViewSource.GetDefaultView(DeviceDataGrid.ItemsSource);
        }

        /// <summary>
        /// Метод слушатель события сохранения 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDeviceSaved()
        {
             MessageBox.Show("Событие сохранения сработало!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information); // Для отладки

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

        public void RefreshContexAndDeviceGrid()
        {
            _dbo = DB_Connection.GetDataBase(); 
            DeviceDataGridInit(_dbo);
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            //_dbo = DB_Connection.GetDataBase(); // Обновляем контекст базы данных на случай если он изменился
            DeviceDataGridInit(_dbo);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedDevice = DeviceDataGrid.SelectedItem as Device;
            if (selectedDevice != null)
            {
                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить это устройство?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    var trackedDevice = _dbo.Device.FirstOrDefault(d => d.DeviceId == selectedDevice.DeviceId);

                    if (trackedDevice != null)
                    {
                        _dbo.Device.Remove(trackedDevice);
                        _dbo.SaveChanges();
                        DeviceDataGridInit(_dbo);
                    }
                }
            }
        }
    }
}
