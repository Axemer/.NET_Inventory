using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
//using System.Windows.Documents; // конфликт
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
        public InventoryRegistryDataBaseEntities3 _dbo = DB_Connection.GetDataBase();

        public MainWindow()
        {
            InitializeComponent();
        }

        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DeviceDataGridInit(_dbo);
        }

        /// <summary>
        /// Забываем список данным из бд
        /// </summary>
        /// <param name="InventoryRegDB"> Данные из БД сюда надо </param>
        private void DeviceDataGridInit(InventoryRegistryDataBaseEntities3 InventoryRegDB)
        {
            DeviceDataGrid.ItemsSource = InventoryRegDB.Device.ToList();
        }

        private void DeviceDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //DeviceDataGrid.SelectedIndex
            //OpenDataWindow();
        }

       
        
        private void DeviceDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var SelctedDevice = DeviceDataGrid.SelectedItem;
          



            if (SelctedDevice != null)
            {
                //OpenDataWindow();
                DataViewerWindow dataViewerWindow = new DataViewerWindow();
                dataViewerWindow.Show();
                dataViewerWindow.ShowData(SelctedDevice, _dbo);
            }
        }
    }
}
