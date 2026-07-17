using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wpf_Inventory_.Classes;
using Wpf_Inventory_.Model;

namespace Wpf_Inventory_.View.Controls
{
    /// <summary>
    /// Логика взаимодействия для OfficeControl.xaml
    /// </summary>
    public partial class OfficeControl : UserControl
    {
        private InventoryDataBaseContext _dbo = DB_Connection.GetDataBase();

        public OfficeControl()
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
            OfficeDataGrid.ItemsSource = InventoryRegDB.Office.ToList();
        }

        /// <summary>
        ///  Добавляет новый офис с базовыми значениями
        /// </summary>
        public void AddNewOffice()
        {
            Office newOffice = new Office
            {
                Officenum = "Новый",
                Phone = ""
            };

            _dbo.Office.Add(newOffice);
            _dbo.SaveChanges();
            DataGridInit(_dbo);
        }

        private void OfficeDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewOffice();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (OfficeDataGrid.SelectedItem is Office office)
            {
                var result = MessageBox.Show($"Удалить офис {office.Officenum}?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    _dbo.Office.Remove(office);
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
    }
}
