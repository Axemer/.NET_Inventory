using Microsoft.EntityFrameworkCore;
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
        private readonly InventoryDataBaseContext _dbo = DB_Connection.GetDataBase();

        public OfficeControl()
        {
            InitializeComponent();
            DataGridInit();

        }

        /// <summary>
        /// Забываем список данным из бд.
        /// </summary>
        /// <param name="InventoryRegDB"> Данные из БД сюда надо </param>
        private void DataGridInit()
        {
            OfficeDataGrid.ItemsSource = _dbo.Office.ToList();
        }


        //<DataGridTextColumn Header = "Блок" Binding="{Binding Block}" Width="auto"/>
        //<DataGridTextColumn Header = "Номер Офиса" Binding="{Binding Officenum}" Width="auto"/>
        //<DataGridTextColumn Header = "Отдел" Binding="{Binding Department}" Width="auto"/>
        //<DataGridTextColumn Header = "Телефон" Binding="{Binding Phone}" Width="auto"/>

        /// <summary>
        ///  Добавляет новый офис с базовыми значениями
        /// </summary>
        public void AddNewOffice()
        {
            // Создаём новый объект Device
            Office newOffice = new Office
            {
                Officenum = "1",
                Phone = "+123",
                Department = "Отдел 0",
                Block = '0'
            };

            // Добавляем в базу данных
            _dbo.Office.Add(newOffice);
            _dbo.SaveChanges(); // Сохраняем в базе, чтобы появился ID тк он присвается базой
            DataGridInit();
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

        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            _dbo.SaveChanges();
            DataGridInit();
        }

        private void OfficeDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Получаем выбранный элемент
            var selectedOffice = OfficeDataGrid.SelectedItem as Office;
            // Проверяем, что элемент не null
            if (selectedOffice != null)
            {
                // Здесь можно выполнить действия с выбранным элементом
                // Например, вывести его данные в текстовые поля или что-то другое
                MessageBox.Show($"Выбран офис: {selectedOffice.Officenum}");
            }
        }

        private void OfficeDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var office = e.Row.Item as Office;

                if (office != null && _dbo.Entry(office).State == EntityState.Detached)
                {
                    _dbo.Office.Add(office);
                }

                _dbo.SaveChanges();
            }
        }
    }
}
