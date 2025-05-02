using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Wpf_Inventory_.Classes;
using Wpf_Inventory_.Model;

namespace Wpf_Inventory_.View
{
    /// <summary>
    /// Логика взаимодействия для DataViewerWindow.xaml
    /// </summary>
    public partial class DataViewerWindow : Window
    {
        /// <summary>
        /// 
        /// </summary>
        public event Action SaveButtonClicked;

        /// <summary>
        /// Ныне выбранное устройство.
        /// </summary>
        private object _currentDevice = new Device();

        /// <summary>
        /// Список всех моделей для фильтрации.
        /// </summary>
        private List<string> _allModels = new List<string>();

        public DataViewerWindow()
        {
            InitializeComponent();
            InitializeModelComboBox();
        }

        /// <summary>
        /// Метод для загрузки всех моделей в ComboBox
        /// </summary>
        private void InitializeModelComboBox()
        {
            InventoryDataBaseContext DBO = DB_Connection.GetDataBase();
            _allModels = DBO.Model.Select(m => m.Model1).Distinct().ToList();
            DevModelComboBox.ItemsSource = _allModels;
            //DevModelComboBox.IsEditable = true;
            //DevModelComboBox.IsTextSearchEnabled = false; // Чтобы мы сами обрабатывали поиск
            //DevModelComboBox.StaysOpenOnEdit = true;

            DevModelComboBox.PreviewKeyUp += DevModelComboBox_PreviewKeyUp;
        }

        /// <summary>
        /// Метод автофильтрации по вводу
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DevModelComboBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            string text = DevModelComboBox.Text.ToLower();
            var filtered = _allModels.Where(m => m.ToLower().Contains(text)).ToList();
            DevModelComboBox.ItemsSource = filtered;
            DevModelComboBox.IsDropDownOpen = true;
        }

        /// <summary>
        /// Сохраняет все внесенные изменения
        /// </summary>
        private void SaveDeviceChanges(object SelectedDevice)
        {
            InventoryDataBaseContext DBO = DB_Connection.GetDataBase();
            if (SelectedDevice == null || DBO == null) return;

            var idProperty = SelectedDevice.GetType().GetProperty("DeviceId");
            if (idProperty == null) return;

            int? deviceId = idProperty.GetValue(SelectedDevice) as int?;
            if (deviceId == null) return;

            var device = DBO.Device
                .Include(d => d.Office)
                .FirstOrDefault(d => d.DeviceId == deviceId);
            if (device == null) return;

            device.Devicename = DevNameTextBox.Text;
            device.Serialnumber = DevSerialTextBox.Text;
            device.Inventorynumber = DevInvNumTextBox.Text;
            device.IpAddress = DevIPTextBox.Text;
            device.Note = DevNoteTextBox.Text;
            device.Dateofcommissioning = DevDateDatePicker.SelectedDate ?? DateTime.Now;
            device.Exception = ExceptionCheckBox.IsChecked;

            // Работа с моделью через DevModelComboBox
            string selectedModel = DevModelComboBox.Text;
            if (!string.IsNullOrEmpty(selectedModel))
            {
                var model = DBO.Model.FirstOrDefault(m => m.Model1 == selectedModel);
                if (model == null)
                {
                    // Если модели нет — создаем новую
                    model = new Model.Model { Model1 = selectedModel };
                    DBO.Model.Add(model);
                    DBO.SaveChanges(); // Сохраняем модель сразу чтобы получить её ID
                }
                device.ModelId = model.ModelId;
            }

            // Тип устройства
            string selectedDeviceType = DevTypeComboBox.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedDeviceType))
                device.DevicetypeId = DBO.Devicetype
                    .FirstOrDefault(d => d.Type == selectedDeviceType)?.DevicetypeId;

            // Офис
            string selectedOffice = DevOfficeComboBox.SelectedItem?.ToString();
            string selectedDepartment = DevDepComboBox.SelectedItem?.ToString();
            string selectedBlock = DevBlockComboBox.SelectedItem?.ToString();
            char? selectedBlockChar = string.IsNullOrWhiteSpace(selectedBlock) ? null : (char?)selectedBlock.FirstOrDefault();

            if (!string.IsNullOrEmpty(selectedDepartment) && !string.IsNullOrEmpty(selectedOffice))
            {
                var officeMatch = DBO.Office.FirstOrDefault(o =>
                    o.Department == selectedDepartment &&
                    o.Officenum == selectedOffice &&
                    (selectedBlockChar == null || o.Block == selectedBlockChar)
                );

                if (officeMatch != null)
                {
                    device.OfficeId = officeMatch.OfficeId;
                }
                else
                {
                    MessageBox.Show("Не удалось найти офис с заданными параметрами (отделение, номер, корпус).");
                }
            }

            try
            {
                DBO.SaveChanges();
                var updatedDevice = DBO.Device
                    .Include(d => d.Office)
                    .FirstOrDefault(d => d.DeviceId == device.DeviceId);
                _currentDevice = updatedDevice;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }

        /// <summary>
        /// Показывает все данне устройства в окне.
        /// </summary>
        public void ShowData(object SelectedDevice, InventoryDataBaseContext DBO)
        {
            if (SelectedDevice == null || DBO == null) return;
            _currentDevice = SelectedDevice;

            PropertyInfo idProperty = SelectedDevice.GetType().GetProperty("DeviceId");
            if (idProperty == null)
                return;

            int deviceId = (int)idProperty.GetValue(SelectedDevice);

            Device device = DBO.Device
                .Include(d => d.Model)
                .Include(d => d.Office)
                .Include(d => d.Devicetype)
                .FirstOrDefault(d => d.DeviceId == deviceId);
            if (device == null)
                return;

            if (DevTypeComboBox.Items.Count == 0)
            {
                foreach (Devicetype deviceType in DBO.Devicetype.ToList())
                    DevTypeComboBox.Items.Add(deviceType.Type);
            }
            if (DevBlockComboBox.Items.Count == 0)
            {
                foreach (Office block in DBO.Office.ToList())
                    DevBlockComboBox.Items.Add(block.Block);
            }
            if (DevDepComboBox.Items.Count == 0)
            {
                foreach (Office department in DBO.Office.ToList())
                    DevDepComboBox.Items.Add(department.Department);
            }
            if (DevOfficeComboBox.Items.Count == 0)
            {
                foreach (Office office in DBO.Office.ToList())
                    DevOfficeComboBox.Items.Add(office.Officenum);
            }

            DevIDTextBox.Text = device.DeviceId.ToString();
            DevNameTextBox.Text = device.Devicename;
            DevSerialTextBox.Text = device.Serialnumber;
            DevInvNumTextBox.Text = device.Inventorynumber;

            //  Показ через DevModelComboBox
            DevModelComboBox.Text = device?.Model?.Model1 ?? "без модели";

            DevIPTextBox.Text = device.IpAddress;
            DevNoteTextBox.Text = device.Note;
            DevDateDatePicker.SelectedDate = device.Dateofcommissioning;
            ExceptionCheckBox.IsChecked = device.Exception;

            DevTypeComboBox.SelectedItem = device.Devicetype?.Type;
            DevBlockComboBox.SelectedItem = device.Office?.Block;
            DevDepComboBox.SelectedItem = device.Office?.Department;
            DevOfficeComboBox.SelectedItem = device.Office?.Officenum;
        }

        private void DevSaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveDeviceChanges(_currentDevice);
            SaveButtonClicked?.Invoke();
        }

        private void ExceptionCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // Пока не используется
        }
    }
}
