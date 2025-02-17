using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using Wpf_Inventory_.Classes;
//using System.Windows.Documents;
using Wpf_Inventory_.dbo;

namespace Wpf_Inventory_.View
{
    /// <summary>
    /// Логика взаимодействия для DataViewerWindow.xaml
    /// </summary>
    public partial class DataViewerWindow : Window
    {
        private Device _currentDevice = new Device();
        private InventoryRegistryDataBaseEntities3 _invDbo = DB_Connection.GetDataBase();

        public DataViewerWindow()
        {
            InitializeComponent();




        }

        public void ShowData(object SelectedDevice, InventoryRegistryDataBaseEntities3 DBO)
        {
            ////Device device = DBO.Device.ToList().FindIndex(GridIndex.);

            //foreach (DeviceType deviceType in DBO.DeviceType.ToList())
            //    DevTypeComboBox.Items.Add(deviceType.Type);

            //foreach (Block block in DBO.Block.ToList())
            //    DevBlockComboBox.Items.Add(block.Block1);

            //foreach (Department department in DBO.Department.ToList())
            //    DevDepComboBox.Items.Add(department.Name);

            //foreach (Office office in DBO.Office.ToList())
            //    DevOfficeComboBox.Items.Add(office.OfficeNum);


            //// тут я начинаю из object выскивать сначала имена переменных потом уже сами переменные
            //// Волшебная 6 
            ////

            //string[] aasd = SelectedDevice.GetType().GetProperties().Select(p => p.Name).ToArray();
            ////DevIDTextBox.Text = aasd[6].ToString(); //GetType().GetProperty(Device_ID).GetValue(SelectedDevice, null).ToString();

            //var obj = new List<object>();

            //foreach (var prop in aasd)
            //{
            //    object PV = SelectedDevice.GetType().GetProperty(prop).GetValue(SelectedDevice, null);

            //    obj.Add(PV);

            //}

            //// Вольшебная 6 это Device_ID потому что всегда в таком порядке раскладывается объект
            //DevIDTextBox.Text = obj[6].ToString();

            //var DevInf = SelectedDevice.GetType().GetProperty(DevIDTextBox.Text);

            if (SelectedDevice == null || DBO == null) return;

            

            // Получаем ID устройства из объекта SelectedDevice (если у него есть свойство Device_ID)
            PropertyInfo idProperty = SelectedDevice.GetType().GetProperty("Device_ID");
            if (idProperty == null)
                return;

            int deviceId = (int)idProperty.GetValue(SelectedDevice);

            // Находим устройство в базе данных по ID
            Device device = DBO.Device.FirstOrDefault(d => d.Device_ID == deviceId);
            if (device == null)
                return;

            // Заполняем выпадающие списки (ComboBox), если они ещё не заполнены
            if (DevTypeComboBox.Items.Count == 0)
            {
                foreach (DeviceType deviceType in DBO.DeviceType.ToList())
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
                    DevOfficeComboBox.Items.Add(office.OfficeNum);
            }

            //this.DataContext = SelectedDevice;

            // Заполняем текстовые поля данными устройства
            DevIDTextBox.Text = device.Device_ID.ToString();
            DevNameTextBox.Text = device.DeviceName.ToString();
            DevSerialTextBox.Text = device.SerialNumber;
            DevInvNumTextBox.Text = device.InventoryNumber;
            DevModelTextBox.Text = device.Model.ToString();
            DevIPTextBox.Text = device.IP_Adress;
            

            // Устанавливаем выбранные элементы в ComboBox
            DevTypeComboBox.SelectedItem = device.DeviceType?.Type;
            DevBlockComboBox.SelectedItem = device.Office.Block;
            DevDepComboBox.SelectedItem = device.Department?.Name;
            DevOfficeComboBox.SelectedItem = device.Office?.OfficeNum;
            DevBlockComboBox.SelectedItem = device.Office.Office_ID;

        }
    }
}
