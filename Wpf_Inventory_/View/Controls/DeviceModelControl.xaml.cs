using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Wpf_Inventory_.Classes;
using Wpf_Inventory_.Model;

namespace Wpf_Inventory_.View.Controls
{
    /// <summary>
    /// Логика взаимодействия для DeviceModelControl.xaml
    /// </summary>
    public partial class DeviceModelControl : UserControl
    {
        /// <summary>
        /// Переменная для взаимодействия с бд
        /// </summary>
        private readonly InventoryDataBaseContext _dbo = DB_Connection.GetDataBase();

        /// <summary>
        /// Представление коллекции для фильтрации данных
        /// </summary>
        private readonly ICollectionView _modelCollectionView;

        public DeviceModelControl()
        {
            InitializeComponent();
            DataGridInit();

            _modelCollectionView = CollectionViewSource.GetDefaultView(ModelDataGrid.ItemsSource);
        }

        /// <summary>
        /// Инициализация списка данных из БД.
        /// </summary>
        private void DataGridInit()
        {
            ModelDataGrid.ItemsSource = _dbo.Model.ToList();
        }

        /// <summary>
        /// Добавляет новую модель устройства с базовыми значениями.
        /// </summary>
        public void AddNewModel()
        {
            Model.Model newModel = new Model.Model
            {
                Model1 = "Новая модель"
            };

            _dbo.Model.Add(newModel);
            _dbo.SaveChanges();
            DataGridInit();
        }

        private void ModelDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ModelDataGrid.SelectedItem is Model.Model model)
            {
                string textToCopy = $"Модель устройства: {model.Model1}";

                Clipboard.SetText(textToCopy);
                MessageBox.Show("Запись скопирована в буфер обмена.", "Копирование", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewModel();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = ModelDataGrid.SelectedItems.Cast<Model.Model>().ToList();

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
                foreach (var model in selectedItems)
                {
                    if (_dbo.Entry(model).State == EntityState.Detached)
                    {
                        _dbo.Model.Attach(model);
                    }

                    _dbo.Model.Remove(model);
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

        private void ModelDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedModel = ModelDataGrid.SelectedItem as Model.Model;
            if (selectedModel != null)
            {
                MessageBox.Show($"Выбрана модель устройства: {selectedModel.Model1}");
            }
        }

        private void ModelDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var model = e.Row.Item as Model.Model;

                if (model != null && _dbo.Entry(model).State == EntityState.Detached)
                {
                    _dbo.Model.Add(model);
                }

                _dbo.SaveChanges();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in ModelDataGrid.Items)
            {
                if (item is Model.Model model)
                {
                    var entry = _dbo.Entry(model);

                    if (entry.State == EntityState.Detached)
                    {
                        _dbo.Model.Add(model);
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        _dbo.Model.Update(model);
                    }
                }
            }
            _dbo.SaveChanges();
            MessageBox.Show("Изменения сохранены.", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string filterText = SearchTextBox.Text.ToLower();

            if (_modelCollectionView != null)
            {
                _modelCollectionView.Filter = item =>
                {
                    if (item is not Model.Model model)
                        return false;

                    // Ищем по полю Model1
                    return (!string.IsNullOrEmpty(model.Model1) && model.Model1.Contains(filterText, StringComparison.CurrentCultureIgnoreCase));
                };

                _modelCollectionView.Refresh();
            }
        }
    }
}

