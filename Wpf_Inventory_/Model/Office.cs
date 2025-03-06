using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Wpf_Inventory_.Model
{
    public partial class Office
    {
        public Office()
        {
            Device = new HashSet<Device>();
            OfficeBlock = new HashSet<OfficeBlock>();
        }

        public int OfficeId { get; set; }
        public string Officenum { get; set; }
        public string Phone { get; set; }

        public virtual ICollection<Device> Device { get; set; }
        public virtual ICollection<OfficeBlock> OfficeBlock { get; set; }
    }
}
