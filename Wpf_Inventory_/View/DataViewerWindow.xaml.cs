using System;
using System.Linq;
using System.Reflection;
using System.Windows;
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
        /// Объявляем событие о нажатии кнопки сохранения
        /// </summary>
        public event Action SaveButtonClicked;

        /// <summary>
        /// Переменная с текущим устройством, которое редактируется в окне.
        /// </summary>
        private object _currentDevice = new Device();

        /// <summary>
        /// Переменная с всем перечнем данных в базе данных.
        /// </summary>
        readonly private InventoryDataBaseContext _invDbo = DB_Connection.GetDataBase();

        public DataViewerWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Сохраняет все внесенные изменения
        /// </summary>
        /// <param name="SelectedDevice"></param>
        /// <param name="DBO"></param>
        private void SaveDeviceChanges(object SelectedDevice, InventoryDataBaseContext DBO)
        {
            if (SelectedDevice == null || DBO == null) return;

            PropertyInfo idProperty = SelectedDevice.GetType().GetProperty("DeviceId");
            if (idProperty == null) return;

            int? deviceId = idProperty.GetValue(SelectedDevice) as int?;
            if (deviceId == null) return;

            Device device = DBO.Device.FirstOrDefault(d => d.DeviceId == deviceId);
            if (device == null) return;

            // Обновляем основные поля устройства
            device.Devicename = DevNameTextBox.Text;
            device.Serialnumber = DevSerialTextBox.Text;
            device.Inventorynumber = DevInvNumTextBox.Text;
            device.IpAddress = DevIPTextBox.Text;
            device.Note = DevNoteTextBox.Text;
            device.Dateofcommissioning = DevDateDatePicker.SelectedDate ?? DateTime.Now;
            device.Exception = ExceptionCheckBox.IsChecked;

            // Обновляем ModelId
            string selectedModel = DevModelTextBox.Text;
            if (!string.IsNullOrEmpty(selectedModel))
                device.ModelId = DBO.Model.FirstOrDefault(m => m.Model1 == selectedModel)?.ModelId;

            // Обновляем связи с другими таблицами
            string selectedDeviceType = DevTypeComboBox.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedDeviceType))
                device.DevicetypeId = DBO.Devicetype.FirstOrDefault(d => d.Type == selectedDeviceType)?.DevicetypeId;

            string selectedDepartment = DevDepComboBox.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedDepartment))
            {
                var officeMatch = DBO.Office.FirstOrDefault(d => d.Department == selectedDepartment);
                if (officeMatch != null)
                {
                    if (device.Office == null)
                    {
                        device.Office = DBO.Office.FirstOrDefault(o => o.OfficeId == device.OfficeId);
                    }
                    if (device.Office != null)
                        device.Office.Department = officeMatch.Department;
                }
                else
                    MessageBox.Show("Не удалось найти отделение с таким названием в базе данных.");
            }

            string selectedOffice = DevOfficeComboBox.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedOffice))
                device.OfficeId = DBO.Office.FirstOrDefault(o => o.Officenum == selectedOffice)?.OfficeId;



            // Сохраняем изменения
            DBO.SaveChanges();
            //MessageBox.Show("Данные сохранены!", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information); // Разадажает
        }

        /// <summary>
        /// Показывает все данне устройства в окне.
        /// </summary>
        /// <param name="SelectedDevice"></param>
        /// <param name="DBO"></param>
        public void ShowData(object SelectedDevice, InventoryDataBaseContext DBO)
        {
            if (SelectedDevice == null || DBO == null) return;
            _currentDevice = SelectedDevice;

            // Получаем ID устройства из объекта SelectedDevice (если у него есть свойство Device_ID)
            PropertyInfo idProperty = SelectedDevice.GetType().GetProperty("DeviceId");
            if (idProperty == null)
                return;

            // Получаем ID устройства для работы с БД
            int deviceId = (int)idProperty.GetValue(SelectedDevice);

            // Находим устройство в базе данных по ID
            Device device = DBO.Device.FirstOrDefault(d => d.DeviceId == deviceId);
            if (device == null)
                return;

            // Заполняем выпадающие списки (ComboBox) если их не забили ранее
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

            // Заполняем текстовые поля данными устройства
            DevIDTextBox.Text = device.DeviceId.ToString();
            DevNameTextBox.Text = device.Devicename.ToString();
            DevSerialTextBox.Text = device.Serialnumber;
            DevInvNumTextBox.Text = device.Inventorynumber;
            DevModelTextBox.Text = device?.Model?.Model1 ?? "Неизвестная модель";
            // на случай если модель null стоит проверка ибо так уже случалось

            DevIPTextBox.Text = device.IpAddress;
            DevNoteTextBox.Text = device.Note;
            DevDateDatePicker.SelectedDate = device.Dateofcommissioning;
            ExceptionCheckBox.IsChecked = device.Exception;

            // Устанавливаем выбранные элементы в ComboBox
            DevTypeComboBox.SelectedItem = device.Devicetype?.Type;
            DevBlockComboBox.SelectedItem = device.Office?.Block;
            DevDepComboBox.SelectedItem = device.Office?.Department;
            DevOfficeComboBox.SelectedItem = device.Office?.Officenum;
        }

        private void DevSaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveDeviceChanges(_currentDevice, _invDbo);
            SaveButtonClicked?.Invoke();
        }

        private void ExceptionCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // Этот метод не обязательный тк в методах выше все и так реализовано
            // Но если очень хочется можно и сюда часть функционала перенести
            // Так как технически это будет более правильно
        }
    }
}

