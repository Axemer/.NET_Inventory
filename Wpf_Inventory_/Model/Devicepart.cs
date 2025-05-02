using System.Collections.Generic;

namespace Wpf_Inventory_.Model
{
    public partial class Devicepart
    {
        public int DevicepartsId { get; set; }

        public string Name { get; set; } = null;

        public virtual ICollection<Device> Devices { get; set; } = new List<Device>();
    }
}
