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
        ///  Добавляет новое устройство с базовыми значениями
        /// </summary>
        public void AddNewDepartment()
        {
            Department newDepartment = new Department
            {
                Name = "NewDep"
            };
            // Добавляем в базу данных
            _dbo.Department.Add(newDepartment);
            _dbo.SaveChanges(); // Сохраняем в базе, чтобы появился ID тк он присвается базой
        }

        /// <summary>
        /// Забываем список данным из бд.
        /// </summary>
        /// <param name="InventoryRegDB"> Данные из БД сюда надо </param>
        public void DataGridInit(InventoryDataBaseContext InventoryRegDB)
        {
            DepartmentDataGrid.ItemsSource = InventoryRegDB.Department.ToList();
        }

        public DepartmentControl()
        {
            InitializeComponent();
            DataGridInit(_dbo);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
