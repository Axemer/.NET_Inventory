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
        public int? DevicetypeId { get; set; }
        public string IpAddress { get; set; }
        public string Devicename { get; set; }
        public DateTime? Dateofcommissioning { get; set; }
        public string Serialnumber { get; set; }
        public string Inventorynumber { get; set; }
        public int? ModelId { get; set; }
        public int? OfficeId { get; set; }
        public bool? Exception { get; set; }
        public string Note { get; set; }

        public virtual Devicetype Devicetype { get; set; }
        public virtual Model Model { get; set; }
        public virtual Office Office { get; set; }
        public virtual ICollection<DeviceWorkplace> DeviceWorkplace { get; set; }
        public virtual ICollection<DevicepartsDevice> DevicepartsDevice { get; set; }
    }
}
