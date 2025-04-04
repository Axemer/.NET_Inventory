using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Wpf_Inventory_.Model
{
    public partial class DeviceWorkplace
    {
        public int DeviceWorkplaceId { get; set; }
        public int WorkplaceId { get; set; }
        public int DeviceId { get; set; }

        public virtual Device Device { get; set; }
        public virtual Workplace Workplace { get; set; }
    }
}
