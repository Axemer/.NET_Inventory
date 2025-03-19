using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using Wpf_Inventory_.Classes;
using Wpf_Inventory_.Model;
using Wpf_Inventory_;
using System.Data.Entity;

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

        private object _currentDevice = new Device();
        private InventoryDataBaseContext _invDbo = DB_Connection.GetDataBase();        

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

            PropertyInfo idProperty = SelectedDevice.GetType().GetProperty("Device_ID");
            if (idProperty == null) return;

            int deviceId = (int)idProperty.GetValue(SelectedDevice);
            Device device = DBO.Device.FirstOrDefault(d => d.DeviceId == deviceId);
            if (device == null) return;

            // Обновляем данные устройства
            device.DeviceName = DevNameTextBox.Text;
            device.SerialNumber = DevSerialTextBox.Text;
            device.InventoryNumber = DevInvNumTextBox.Text;
            device.IpAddress = DevIPTextBox.Text; 
            device.Note = DevNoteTextBox.Text;
            device.DateOfCommissioning = DevDateDatePicker.SelectedDate ?? DateTime.Now;

            // Обновляем Model_ID ибо на прямую не хочет
            string selectedModel = DevModelTextBox.Text;
            if (!string.IsNullOrEmpty(selectedModel))
            {
                Model.Model model = DBO.Model.FirstOrDefault(m => m.Model1 == selectedModel);
                if (model != null)
                {
                    device.ModelId = model.ModelId; //Сохраняем ID модели
                    device.Model = model;
                }
            }

            // Обновляем связи с другими таблицами
            string selectedDeviceType = DevTypeComboBox.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedDeviceType))
                device.Devicetype = DBO.Devicetype.FirstOrDefault(d => d.Type == selectedDeviceType);

            string selectedDepartment = DevDepComboBox.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedDepartment))
                device.Department = DBO.Department.FirstOrDefault(d => d.Name == selectedDepartment);

            string selectedOffice = DevOfficeComboBox.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedOffice))
                device.Office = DBO.Office.FirstOrDefault(o => o.Officenum == selectedOffice);

            // Потуги не идееспособные по идее ибо результата нет
            if (device.Office != null)
            {
                string selectedBlock = DevBlockComboBox.SelectedItem?.ToString();
                if (!string.IsNullOrEmpty(selectedBlock))
                {
                    Block block = DBO.Block.FirstOrDefault(b => b.Block1 == selectedBlock);
                    if (block != null)
                    {
                        // ощибки 
                        //device.Office.Block.Clear(); // Удаляем старые связи, если требуется
                        //device.Office.Block.Add(block); // Добавляем новый блок
                    }
                }
            }

            // Сохраняем изменения
            DBO.SaveChanges();
            MessageBox.Show("Данные сохранены!", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);

            // Вызываем событие после сохранения
            
        }

        /// <summary>
        /// Показывает все данне устройства в окне.
        /// </summary>
        /// <param name="SelectedDevice"></param>
        /// <param name="DBO"></param>
        public void ShowData(object SelectedDevice, InventoryDataBaseContext DBO)
        {
            //DBO.Configuration.ProxyCreationEnabled = false;

            if (SelectedDevice == null || DBO == null) return;
            _currentDevice = SelectedDevice;

            // Получаем ID устройства из объекта SelectedDevice (если у него есть свойство Device_ID)
            PropertyInfo idProperty = SelectedDevice.GetType().GetProperty("Device_ID");
            if (idProperty == null)
                return;

            int deviceId = (int)idProperty.GetValue(SelectedDevice);

            // Находим устройство в базе данных по ID
            Device device = DBO.Device.FirstOrDefault(d => d.DeviceId == deviceId);
            if (device == null)
                return;

            // Заполняем выпадающие списки (ComboBox), если они ещё не заполнены
            if (DevTypeComboBox.Items.Count == 0)
            {
                foreach (DeviceType deviceType in DBO.Devicetype.ToList())
                    DevTypeComboBox.Items.Add(deviceType.Type);
            }

            if (DevBlockComboBox.Items.Count == 0)
            {
                foreach (Block block in DBO.Block.ToList())
                    DevBlockComboBox.Items.Add(block.Block1);
            }

            if (DevDepComboBox.Items.Count == 0)
            {
                foreach (Department department in DBO.Department.ToList())
                    DevDepComboBox.Items.Add(department.Name);
            }

            if (DevOfficeComboBox.Items.Count == 0)
            {
                foreach (Office office in DBO.Office.ToList())
                    DevOfficeComboBox.Items.Add(office.Officenum);
            }

            // Заполняем текстовые поля данными устройства
            //DevIDTextBox.Text = device.Device_ID.ToString();
            DevNameTextBox.Text = device.DeviceName.ToString();
            DevSerialTextBox.Text = device.SerialNumber;
            DevInvNumTextBox.Text = device.InventoryNumber;
            DevModelTextBox.Text = device?.Model?.Model1 ?? "Неизвестная модель";
            DevIPTextBox.Text = device.IpAddress;
            DevNoteTextBox.Text = device.Note;
            DevDateDatePicker.SelectedDate = device.DateOfCommissioning;

            // Устанавливаем выбранные элементы в ComboBox
            DevTypeComboBox.SelectedItem = device.Devicetype?.Type;
            DevBlockComboBox.SelectedItem = device.Office.OfficeBlock;
            DevDepComboBox.SelectedItem = device.Department?.Name;
            DevOfficeComboBox.SelectedItem = device.Office?.Officenum;            

        }

        private void DevSaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveDeviceChanges(_currentDevice, _invDbo);
            SaveButtonClicked?.Invoke();
        }
    }
}

