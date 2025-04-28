using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Wpf_Inventory_.Classes;
using Wpf_Inventory_.Model;

namespace Wpf_Inventory_.View.Controls
{
    /// <summary>
    /// Логика взаимодействия для OfficeControl.xaml
    /// </summary>
    public partial class DeviceTypeControl : UserControl
    {
        /// <summary>
        /// Переменная для взаимодействия с бд
        /// </summary>
        private readonly InventoryDataBaseContext _dbo = DB_Connection.GetDataBase();

        /// <summary>
        /// Представление коллекции для фильтрации данных
        /// </summary>
        private ICollectionView _deviceTypeCollectionView;

        public DeviceTypeControl()
        {
            InitializeComponent();
            DataGridInit();
        }

        /// <summary>
        /// Инициализация списка данных из БД.
        /// </summary>
        private void DataGridInit()
        {
            DeviceTypeDataGrid.ItemsSource = _dbo.Devicetype.ToList();
            _deviceTypeCollectionView = CollectionViewSource.GetDefaultView(DeviceTypeDataGrid.ItemsSource);
        }

        /// <summary>
        /// Добавляет новый тип устройства с базовыми значениями.
        /// </summary>
        public void AddNewDeviceType()
        {
            Devicetype newDeviceType = new Devicetype
            {
                Type = "Новый тип устройства"
            };

            _dbo.Devicetype.Add(newDeviceType);
            _dbo.SaveChanges();
            DataGridInit();
        }

        private void DeviceTypeDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DeviceTypeDataGrid.SelectedItem is Devicetype deviceType)
            {
                string textToCopy = $"Тип устройства: {deviceType.Type}";

                Clipboard.SetText(textToCopy);
                MessageBox.Show("Запись скопирована в буфер обмена.", "Копирование", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewDeviceType();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = DeviceTypeDataGrid.SelectedItems.Cast<Devicetype>().ToList();

            if (selectedItems.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы одну запись для удаления.", "Удаление", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Вы действительно хотите удалить {selectedItems.Count} запись(ей)?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                foreach (var deviceType in selectedItems)
                {
                    if (_dbo.Entry(deviceType).State == EntityState.Detached)
                    {
                        _dbo.Devicetype.Attach(deviceType);
                    }

                    _dbo.Devicetype.Remove(deviceType);
                }

                _dbo.SaveChanges();
                DataGridInit();
                MessageBox.Show("Удаление выполнено.", "Удаление", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            _dbo.SaveChanges();
            DataGridInit();
        }

        private void DeviceTypeDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedDeviceType = DeviceTypeDataGrid.SelectedItem as Devicetype;
            if (selectedDeviceType != null)
            {
                MessageBox.Show($"Выбран тип устройства: {selectedDeviceType.Type}");
            }
        }

        private void DeviceTypeDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var deviceType = e.Row.Item as Devicetype;

                if (deviceType != null && _dbo.Entry(deviceType).State == EntityState.Detached)
                {
                    _dbo.Devicetype.Add(deviceType);
                }

                _dbo.SaveChanges();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in DeviceTypeDataGrid.Items)
            {
                if (item is Devicetype deviceType)
                {
                    var entry = _dbo.Entry(deviceType);

                    if (entry.State == EntityState.Detached)
                    {
                        _dbo.Devicetype.Add(deviceType);
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        _dbo.Devicetype.Update(deviceType);
                    }
                }
            }
            _dbo.SaveChanges();
            MessageBox.Show("Изменения сохранены.", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string filterText = SearchTextBox.Text.ToLower();

            if (_deviceTypeCollectionView != null)
            {
                _deviceTypeCollectionView.Filter = item =>
                {
                    if (item is not Devicetype deviceType)
                        return false;

                    // Ищем просто по тексту во всех полях, которые есть
                    return (!string.IsNullOrEmpty(deviceType.Type) && deviceType.Type.Contains(filterText, StringComparison.CurrentCultureIgnoreCase));
                };

                _deviceTypeCollectionView.Refresh();
            }
        }

    }
}
