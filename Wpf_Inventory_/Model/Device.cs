using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Wpf_Inventory_.Model
{
    public partial class Device
    {
        public Device()
        {
            DeviceWorkplace = new HashSet<DeviceWorkplace>();
            DevicepartsDevice = new HashSet<DevicepartsDevice>();
        }

        public int DeviceId { get; set; }
        public int? DeviceTypeId { get; set; }
        public string IpAddress { get; set; }
        public int DepartmentId { get; set; }
        public string DeviceName { get; set; }
        public DateTime? DateOfCommissioning { get; set; }
        public string SerialNumber { get; set; }
        public string InventoryNumber { get; set; }
        public int? ModelId { get; set; }
        public int? OfficeId { get; set; }
        public bool? Exception { get; set; }
        public string Note { get; set; }

        public virtual Department Department { get; set; }
        public virtual DeviceType Devicetype { get; set; }
        public virtual Model Model { get; set; }
        public virtual Office Office { get; set; }
        public virtual ICollection<DeviceWorkplace> DeviceWorkplace { get; set; }
        public virtual ICollection<DevicepartsDevice> DevicepartsDevice { get; set; }
    }
}
