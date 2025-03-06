using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Wpf_Inventory_.Model
{
    public partial class DevicepartsDevice
    {
        public int DeviceId { get; set; }
        public int DevicepartsId { get; set; }

        public virtual Device Device { get; set; }
        public virtual Deviceparts Deviceparts { get; set; }
    }
}
