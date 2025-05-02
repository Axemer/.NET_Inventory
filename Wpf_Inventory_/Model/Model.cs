using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Wpf_Inventory_.Model
{
    public partial class Model
    {
        public Model()
        {
            Device = new HashSet<Device>();
        }

        public int ModelId { get; set; }
        public string Model1 { get; set; }

        public virtual ICollection<Device> Device { get; set; }
    }
}
