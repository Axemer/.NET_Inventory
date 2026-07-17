using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Wpf_Inventory_.Classes;
using Wpf_Inventory_.Model;

namespace Wpf_Inventory_.View.Controls
{
    /// <summary>
    /// Логика взаимодействия для DeviceTypeControl.xaml
    /// </summary>
    public partial class DeviceTypeControl : UserControl
    {
        /// <summary>
        /// переменная для взаимодействия с бд
        /// </summary>
        private InventoryDataBaseContext _dbo = DB_Connection.GetDataBase();

        public DeviceTypeControl()
        {
            InitializeComponent();
            DataGridInit(_dbo);
        }

        /// <summary>
        /// Забываем список данным из бд.
        /// </summary>
        /// <param name="InventoryRegDB"> Данные из БД сюда надо </param>
        private void DataGridInit(InventoryDataBaseContext InventoryRegDB)
        {
            TypeDataGrid.ItemsSource = InventoryRegDB.Devicetype.ToList();
        }

        /// <summary>
        ///  Добавляет новый тип устройства
        /// </summary>
        public void AddNew()
        {
            Devicetype newType = new()
            {
                Type = "Новый тип"
            };

            _dbo.Devicetype.Add(newType);
            _dbo.SaveChanges();
            DataGridInit(_dbo);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddNew();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (TypeDataGrid.SelectedItem is Devicetype devType)
            {
                var result = MessageBox.Show($"Удалить тип \"{devType.Type}\"?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    _dbo.Devicetype.Remove(devType);
                    _dbo.SaveChanges();
                    DataGridInit(_dbo);
                }
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            _dbo.SaveChanges();
            DataGridInit(_dbo);
        }

        private void TypeDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

    }
}
