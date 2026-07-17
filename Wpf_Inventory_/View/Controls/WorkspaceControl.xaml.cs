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
        public InventoryDataBaseContext _dbo = DB_Connection.GetDataBase();

        public WorkspaceControl()
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
            WorkspaceDataGrid.ItemsSource = InventoryRegDB.Workplace.ToList();
        }

        /// <summary>
        ///  Добавляет новое рабочее место
        /// </summary>
        public void AddNew()
        {
            Workplace newWorkplace = new()
            {
                WorkplaceNote = "Новое рабочее место"
            };

            _dbo.Workplace.Add(newWorkplace);
            _dbo.SaveChanges();
            DataGridInit(_dbo);
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
            if (WorkspaceDataGrid.SelectedItem is Workplace workplace)
            {
                var result = MessageBox.Show($"Удалить рабочее место?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    _dbo.Workplace.Remove(workplace);
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
