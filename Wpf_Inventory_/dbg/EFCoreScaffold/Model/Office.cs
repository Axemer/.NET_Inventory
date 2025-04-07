using System;
using System.Collections.Generic;

namespace EFCoreScaffold.Model;

public partial class Office
{
    public int OfficeId { get; set; }

    public string Officenum { get; set; } = null!;

    public string? Phone { get; set; }

    public char? Block { get; set; }

    public string? Department { get; set; }

    public virtual ICollection<Device> Devices { get; set; } = new List<Device>();
}
