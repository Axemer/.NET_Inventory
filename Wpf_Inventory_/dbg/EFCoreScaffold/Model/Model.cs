using System;
using System.Collections.Generic;

namespace EFCoreScaffold.Model;

public partial class Model
{
    public int ModelId { get; set; }

    public string Model1 { get; set; } = null!;

    public virtual ICollection<Device> Devices { get; set; } = new List<Device>();
}
