// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Wpf_Inventory_.Model
{
    public partial class OfficeBlock
    {
        public int OfficeId { get; set; }
        public int BlockId { get; set; }

        public virtual Block Block { get; set; }
        public virtual Office Office { get; set; }
    }
}
