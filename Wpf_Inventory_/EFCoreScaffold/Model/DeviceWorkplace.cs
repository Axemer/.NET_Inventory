using System;
using System.Collections.Generic;

namespace EFCoreScaffold.Model;

public partial class DeviceWorkplace
{
    public int DeviceWorkplaceId { get; set; }

    public int WorkplaceId { get; set; }

    public int DeviceId { get; set; }

    public virtual Device Device { get; set; } = null!;

    public virtual Workplace Workplace { get; set; } = null!;
}
