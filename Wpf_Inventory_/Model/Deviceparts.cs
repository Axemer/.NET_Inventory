using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Wpf_Inventory_.Model
{
    public partial class Deviceparts
    {
        public Deviceparts()
        {
            DevicepartsDevice = new HashSet<DevicepartsDevice>();
        }

        public int DevicepartsId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<DevicepartsDevice> DevicepartsDevice { get; set; }
    }
}
