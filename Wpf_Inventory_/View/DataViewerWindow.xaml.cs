using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
//using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Wpf_Inventory_.dbo;
using Wpf_Inventory_.Classes;
using System.Data.Entity;

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
            //Device device = DBO.Device.ToList().FindIndex(GridIndex.);

            foreach (DeviceType deviceType in DBO.DeviceType.ToList())
                DevTypeComboBox.Items.Add(deviceType.Type);

            foreach (Block block in DBO.Block.ToList())
                DevBlockComboBox.Items.Add(block.Block1);

            foreach (Department department in DBO.Department.ToList())
                DevDepComboBox.Items.Add(department.Name);

            foreach (Office office in DBO.Office.ToList())
                DevOfficeComboBox.Items.Add(office.OfficeNum);

           
            // тут я начинаю из object выскивать сначала имена переменных потом уже сами переменные
            // Волшебная 6 
            //

            string[] aasd = SelectedDevice.GetType().GetProperties().Select(p => p.Name).ToArray();
            //DevIDTextBox.Text = aasd[6].ToString(); //GetType().GetProperty(Device_ID).GetValue(SelectedDevice, null).ToString();

            var obj = new List<object>();

            foreach (var prop in aasd)
            {
                object PV =  SelectedDevice.GetType().GetProperty(prop).GetValue(SelectedDevice, null);

                obj.Add(PV);

            }

            // Вольшебная 6 это Device_ID потому что всегда в таком порядке раскладывается объект
            DevIDTextBox.Text = obj[6].ToString();

            var DevInf = SelectedDevice.GetType().GetProperty(DevIDTextBox.Text);
            


        }
    }
}
