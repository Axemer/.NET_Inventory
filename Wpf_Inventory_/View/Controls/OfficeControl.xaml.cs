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
            if (OfficeDataGrid.SelectedItem is Office office)
            {
                string textToCopy = $"ID: {office.OfficeId}\n" +
                                    $"Номер: {office.Officenum}\n" +
                                    $"Телефон: {office.Phone}\n" +
                                    $"Отдел: {office.Department}\n" +
                                    $"Блок: {office.Block}";

                Clipboard.SetText(textToCopy);
                MessageBox.Show("Запись скопирована в буфер обмена.", "Копирование", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewOffice();
        }

        /// <summary>
        /// Обработчик события нажатия кнопки "Удалить".
        /// С логикой удаления выделенных записей из DataGrid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = OfficeDataGrid.SelectedItems.Cast<Office>().ToList();

            if (selectedItems.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы одну запись для удаления.", "Удаление", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Вы действительно хотите удалить {selectedItems.Count} запись(ей)?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                foreach (var office in selectedItems)
                {
                    if (_dbo.Entry(office).State == EntityState.Detached)
                    {
                        _dbo.Office.Attach(office);
                    }

                    _dbo.Office.Remove(office);
                }

                _dbo.SaveChanges();
                DataGridInit();
                MessageBox.Show("Удаление выполнено.", "Удаление", MessageBoxButton.OK, MessageBoxImage.Information);
            }
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

        /// <summary>
        /// Обработчик события, который срабатывает при завершении редактирования строки в DataGrid.
        /// Работает нормально но не очевидно и не всегда
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Обработчик события нажатия кнопки "Сохранить".
        /// с логикой сохранения изменений в DataGrid.
        /// 
        /// Зачем один метод сохранение если можно целых два.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in OfficeDataGrid.Items)
            {
                if (item is Office office)
                {
                    var entry = _dbo.Entry(office);

                    if (entry.State == EntityState.Detached)
                    {
                        _dbo.Office.Add(office); // Новая запись
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        _dbo.Office.Update(office); // Изменённая запись
                    }
                }
            }

            _dbo.SaveChanges();
            MessageBox.Show("Изменения сохранены.", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
