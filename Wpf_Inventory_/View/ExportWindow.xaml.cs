using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Wpf_Inventory_.Classes;
using Wpf_Inventory_.Model;

namespace Wpf_Inventory_.View
{
    /// <summary>
    /// Логика взаимодействия для ExportWindow.xaml
    /// </summary>
    public partial class ExportWindow : Window
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IEnumerable<Device> _visibleDevices;

        /// <summary>
        /// 
        /// </summary>
        private readonly InventoryDataBaseContext _dbContext = DB_Connection.GetDataBase();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="visibleDevices"></param>
        /// <param name="dbContext"></param>
        public  ExportWindow(IEnumerable<Device> visibleDevices)
        {
            InitializeComponent();
            _visibleDevices = visibleDevices;
        }

        private void ExecuteExport_Click(object sender, RoutedEventArgs e)
        {
            bool exportAll = ExportAllRadio.IsChecked == true;
            DateTime? dateFrom = DateFromPicker.SelectedDate;
            DateTime? dateTo = DateToPicker.SelectedDate;

            if (exportAll)
            {
                ExportAllDevices(dateFrom, dateTo);
            }
            else
            {
                ExportVisibleDevices(dateFrom, dateTo);
            }

            MessageBox.Show("Экспорт завершён.", "Экспорт", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ExportAllDevices(DateTime? dateFrom, DateTime? dateTo)
        {
            var devices = _dbContext.Device
                .Include(d => d.Devicetype)
                .AsNoTracking()
                .ToList();

            if (dateFrom.HasValue)
                devices = devices.Where(d => d.Dateofcommissioning >= dateFrom.Value).ToList();

            if (dateTo.HasValue)
                devices = devices.Where(d => d.Dateofcommissioning <= dateTo.Value).ToList();

            if (devices.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта (все устройства).", "Экспорт", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            RunExport(devices);
        }

        private void ExportVisibleDevices(DateTime? dateFrom, DateTime? dateTo)
        {
            var devices = _visibleDevices.ToList();

            if (dateFrom.HasValue)
                devices = devices.Where(d => d.Dateofcommissioning >= dateFrom.Value).ToList();

            if (dateTo.HasValue)
                devices = devices.Where(d => d.Dateofcommissioning <= dateTo.Value).ToList();

            if (devices.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта (видимые устройства).", "Экспорт", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            RunExport(devices);
        }

        private void RunExport(List<Device> devices)
        {
            // Здесь просто пример:
            foreach (var device in devices)
            {
                Console.WriteLine($"Экспортируем: {device.Devicename}, {device.Inventorynumber}");
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Флаг, указывающий на необходимость экспорта только видимых элементов.
        /// </summary>
        ///public bool ExportAll { get; private set; }

        /// <summary>
        /// Дата от которой будет осуществляться экспорт.
        /// </summary>
        //public DateTime? DateFrom { get; private set; }

        /// <summary>
        /// Дата до которой будет осуществляться экспорт.
        /// </summary>
        //public DateTime? DateTo { get; private set; }

        //private void ExportAllDevices(DateTime? dateFrom, DateTime? dateTo)
        //{
        //    using var db = new InventoryDataBaseContext();

        //    var devices = db.Device
        //        .Include(d => d.Devicetype)
        //        .AsNoTracking()
        //        .ToList();

        //    if (dateFrom.HasValue)
        //        devices = devices.Where(d => d.Dateofcommissioning >= dateFrom.Value).ToList();

        //    if (dateTo.HasValue)
        //        devices = devices.Where(d => d.Dateofcommissioning <= dateTo.Value).ToList();

        //    if (devices.Count == 0)
        //    {
        //        MessageBox.Show("Нет данных для экспорта.", "Экспорт", MessageBoxButton.OK, MessageBoxImage.Information);
        //        return;
        //    }

        //    // Твоя логика экспорта сюда (например, экспорт в Excel/CSV)
        //    Debug.WriteLine($"Экспортировано ВСЕГО: {devices.Count} устройств.");
        //}

        
        //private void ExportVisibleDevices(DateTime? dateFrom, DateTime? dateTo)
        //{
        //    var visibleDevices = _deviceCollectionView.Cast<Device>().ToList();

        //    if (dateFrom.HasValue)
        //        visibleDevices = visibleDevices.Where(d => d.Dateofcommissioning >= dateFrom.Value).ToList();

        //    if (dateTo.HasValue)
        //        visibleDevices = visibleDevices.Where(d => d.Dateofcommissioning <= dateTo.Value).ToList();

        //    if (visibleDevices.Count == 0)
        //    {
        //        MessageBox.Show("Нет данных для экспорта.", "Экспорт", MessageBoxButton.OK, MessageBoxImage.Information);
        //        return;
        //    }

        //    // Твоя логика экспорта сюда (например, экспорт в Excel/CSV)
        //    Debug.WriteLine($"Экспортировано ВИДИМЫХ: {visibleDevices.Count} устройств.");
        //}



        //public ExportWindow()
        //{
        //    InitializeComponent();
        //}

        //private void ExportVisible_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void ExportAll_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void ExecuteExport_Click(object sender, RoutedEventArgs e)
        //{
        //    ExportAll = ExportAllRadio.IsChecked == true;
        //    DateFrom = DateFromPicker.SelectedDate;
        //    DateTo = DateToPicker.SelectedDate;

        //    // Закрываем окно с результатом OK
        //    this.DialogResult = true;
        //    this.Close();
        //}

        //private void Cancel_Click(object sender, RoutedEventArgs e)
        //{
        //    this.DialogResult = false;
        //    this.Close();
        //}
    }
}
