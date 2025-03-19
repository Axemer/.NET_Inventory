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
        ///  Добавляет новый офис с базовыми значениями
        /// </summary>
        public void AddNew()
        {
            // Создаём новый объект Device
            Office newOffice = new Office
            {
                //Block = null, // ???? хз сюда ничего кроме блока и не вставиь 
                //OfficeNum = "1",
                Phone = "+123"
            };

            // Добавляем в базу данных
            _dbo.Office.Add(newOffice);
            _dbo.SaveChanges(); // Сохраняем в базе, чтобы появился ID тк он присвается базой
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

        private void TypeDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

    }
}
