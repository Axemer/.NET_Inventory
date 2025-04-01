using System;
using System.Collections.Generic;
using System.Data.Entity;
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

            PropertyInfo idProperty = SelectedDevice.GetType().GetProperty("DeviceId");
            if (idProperty == null) return;

            int? deviceId = idProperty.GetValue(SelectedDevice) as int?;
            if (deviceId == null) return;

            Device device = DBO.Device.FirstOrDefault(d => d.DeviceId == deviceId);
            if (device == null) return;

            // Обновляем основные поля устройства
            device.DeviceName = DevNameTextBox.Text;
            device.SerialNumber = DevSerialTextBox.Text;
            device.InventoryNumber = DevInvNumTextBox.Text;
            device.IpAddress = DevIPTextBox.Text;
            device.Note = DevNoteTextBox.Text;
            device.DateOfCommissioning = DevDateDatePicker.SelectedDate ?? DateTime.Now;
            device.Exception = ExceptionCheckBox.IsChecked;

            // Обновляем ModelId
            string selectedModel = DevModelTextBox.Text;
            if (!string.IsNullOrEmpty(selectedModel))
                device.ModelId = DBO.Model.FirstOrDefault(m => m.Model1 == selectedModel)?.ModelId;

            // Обновляем связи с другими таблицами
            string selectedDeviceType = DevTypeComboBox.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedDeviceType))
                device.DeviceTypeId = DBO.Devicetype.FirstOrDefault(d => d.Type == selectedDeviceType)?.DevicetypeId;

            string selectedDepartment = DevDepComboBox.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedDepartment))
                device.DepartmentId = DBO.Department.FirstOrDefault(d => d.Name == selectedDepartment).DepartmentId;

            string selectedOffice = DevOfficeComboBox.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedOffice))
                device.OfficeId = DBO.Office.FirstOrDefault(o => o.Officenum == selectedOffice)?.OfficeId;

            // Обновляем связь с блоком 
            // Говорили они так лучше ме ме ме 
            // Говорили меньше места занимает ме ме ме
            // Было бы у меня столько ебли с людим как с этими блоками
            // Я бы стал Хью Хефнером 2.0 и был бы не менее знаменит.
            string selectedBlock = DevBlockComboBox.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedBlock) && device.OfficeId != null)
            {
                using (var newContext = new InventoryDataBaseContext()) // Отдельный контекст для обновления связи
                {
                    var block = newContext.Block.AsNoTracking().FirstOrDefault(b => b.Block1 == selectedBlock);
                    if (block != null)
                    {
                        // Ищем старую связь
                        var oldOfficeBlock = newContext.OfficeBlock
                            .Where(ob => ob.OfficeId == device.OfficeId)
                            .ToList(); // Загружаем все связи для этого офиса

                        if (oldOfficeBlock.Any())
                        {
                            newContext.OfficeBlock.RemoveRange(oldOfficeBlock); // Удаляем все старые связи
                            newContext.SaveChanges();
                        }

                        // Добавляем новую связь
                        var newOfficeBlock = new OfficeBlock
                        {
                            OfficeId = device.OfficeId.Value,
                            BlockId = block.BlockId
                        };

                        newContext.OfficeBlock.Add(newOfficeBlock);
                        newContext.SaveChanges(); // Фиксируем изменения
                    }
                }
                // Загружаем данные снова, чтобы обновить UI без отслеживания старых связей
                DBO.Entry(device).Reload();
            }

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
            //DBO.Configuration.ProxyCreationEnabled = false;

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
            DevIDTextBox.Text = device.DeviceId.ToString();
            DevNameTextBox.Text = device.DeviceName.ToString();
            DevSerialTextBox.Text = device.SerialNumber;
            DevInvNumTextBox.Text = device.InventoryNumber;
            DevModelTextBox.Text = device?.Model?.Model1 ?? "Неизвестная модель"; // сломано не фурычит
            DevIPTextBox.Text = device.IpAddress;
            DevNoteTextBox.Text = device.Note;
            DevDateDatePicker.SelectedDate = device.DateOfCommissioning;
            ExceptionCheckBox.IsChecked = device.Exception;

            // Устанавливаем выбранные элементы в ComboBox
            DevTypeComboBox.SelectedItem = device.Devicetype?.Type;
            //DevBlockComboBox.SelectedItem = device.Office?.OfficeBlock?.FirstOrDefault(ob => ob.OfficeId == device.OfficeId)?.Block?.Block1;
            DevDepComboBox.SelectedItem = device.Department?.Name;
            DevOfficeComboBox.SelectedItem = device.Office?.Officenum;

            // Поиск блока через OfficeBlock
            var officeBlock = DBO.OfficeBlock.Include(ob => ob.Block)
                                             .FirstOrDefault(ob => ob.OfficeId == device.OfficeId);
            if (officeBlock != null)
            {
                DevBlockComboBox.SelectedItem = officeBlock.Block.Block1;
            }
        }

        private void DevSaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveDeviceChanges(_currentDevice, _invDbo);
            SaveButtonClicked?.Invoke();
        }

        private void ExceptionCheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}

