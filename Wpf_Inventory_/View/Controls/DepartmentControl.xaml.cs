using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Wpf_Inventory_.Classes;
using Wpf_Inventory_.Model;

namespace Wpf_Inventory_.View.Controls
{
    /// <summary>
    /// Логика взаимодействия для DepartmentControl.xaml
    /// </summary>
    public partial class DepartmentControl : UserControl
    {
        /// <summary>
        /// Переменная с всем перечнем данных в базе данных.
        /// </summary>
        public InventoryDataBaseContext _dbo = DB_Connection.GetDataBase();

        /// <summary>
        ///  Добавляет новый отдел с базовыми значениями
        /// </summary>
        public void AddNewDepartment()
        {
            Office newDepartment = new Office
            {
                Officenum = "Новый",
                Department = "Новый отдел"
            };
            _dbo.Office.Add(newDepartment);
            _dbo.SaveChanges();
            DataGridInit(_dbo);
        }

        /// <summary>
        /// Забываем список данным из бд.
        /// </summary>
        /// <param name="InventoryRegDB"> Данные из БД сюда надо </param>
        public void DataGridInit(InventoryDataBaseContext InventoryRegDB)
        {
            DepartmentDataGrid.ItemsSource = InventoryRegDB.Office.ToList();
        }

        public DepartmentControl()
        {
            InitializeComponent();
            DataGridInit(_dbo);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewDepartment();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            _dbo.SaveChanges();
            DataGridInit(_dbo);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (DepartmentDataGrid.SelectedItem is Office department)
            {
                var result = MessageBox.Show($"Удалить отдел \"{department.Department}\"?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    _dbo.Office.Remove(department);
                    _dbo.SaveChanges();
                    DataGridInit(_dbo);
                }
            }
        }
    }
}
