using System;
using System.Collections.Generic;

namespace EFCoreScaffold.Model;

public partial class Workplace
{
    public int WorkplaceId { get; set; }

    public string? WorkplaceNote { get; set; }

    public virtual ICollection<DeviceWorkplace> DeviceWorkplaces { get; set; } = new List<DeviceWorkplace>();
}
