using System.Collections.Generic;
using Wpf_Inventory_.Model;

namespace Wpf_Inventory_.Classes
{
    internal class DatabaseSnapshot
    {
        public List<Device> Devices { get; set; } = new();
        public List<Devicetype> Devicetypes { get; set; } = new();
        public List<Deviceparts> Deviceparts { get; set; } = new();
        public List<Office> Offices { get; set; } = new();
        public List<DeviceWorkplace> DeviceWorkplaces { get; set; } = new();
        public List<DevicepartsDevice> DevicepartsDevices { get; set; } = new();
        public List<Model.Model> Models { get; set; } = new();
        public List<Workplace> Workplaces { get; set; } = new();
    }
}
