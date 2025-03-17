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
    /// Логика взаимодействия для DepartmentControl.xaml
    /// </summary>
    public partial class DepartmentControl : UserControl
    {
        /// <summary>
        /// Переменная с всем перечнем данных в базе данных.
        /// </summary>
        public InventoryRegistryDataBaseEntities3 _dbo = DB_Connection.GetDataBase();

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
        public void DataGridInit(InventoryRegistryDataBaseEntities3 InventoryRegDB)
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
