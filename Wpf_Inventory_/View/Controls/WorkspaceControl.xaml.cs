using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wpf_Inventory_.Classes;
using Wpf_Inventory_.Model;


namespace Wpf_Inventory_.View.Controls
{
    /// <summary>
    /// Логика взаимодействия для WorkspaceControl.xaml
    /// </summary>
    public partial class WorkspaceControl : UserControl
    {
        /// <summary>
        /// Переменная с всем перечнем данных в базе данных.
        /// </summary>
        public InventoryDataBaseContext _dbo; //= DB_Connection.GetDataBase();

        public WorkspaceControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Забываем список данным из бд.
        /// </summary>
        /// <param name="InventoryRegDB"> Данные из БД сюда надо </param>
        private void DataGridInit(InventoryDataBaseContext InventoryRegDB)
        {
            WorkspaceDataGrid.ItemsSource = InventoryRegDB.Device.ToList();
        }

        /// <summary>
        ///  Добавляет новый офис с базовыми значениями
        /// </summary>
        public void AddNew()
        {
            // Создаём новый объект Device
            Office newOffice = new Office
            {
                //Block = null, // ???? хз сюда ничего кроме блока и не вставиь 
                Officenum = "1",
                Phone = "+123"
            };

            // Добавляем в базу данных
            _dbo.Office.Add(newOffice);
            _dbo.SaveChanges(); // Сохраняем в базе, чтобы появился ID тк он присвается базой
        }

        private void WorkspaceDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void WorkspaceDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Your double-click logic here
        }
        private void OfficeDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddNew();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            _dbo.SaveChanges();
            DataGridInit(_dbo);
        }
    }
}
