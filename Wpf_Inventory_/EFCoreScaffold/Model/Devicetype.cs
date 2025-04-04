using System;
using System.Collections.Generic;

namespace EFCoreScaffold.Model;

public partial class Devicetype
{
    public int DevicetypeId { get; set; }

    public string Type { get; set; } = null!;

    public virtual ICollection<Device> Devices { get; set; } = new List<Device>();
}
