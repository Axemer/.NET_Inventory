using System;
using System.Collections.Generic;

namespace EFCoreScaffold.Model;

public partial class Device
{
    public int DeviceId { get; set; }

    public int? DevicetypeId { get; set; }

    public string? IpAddress { get; set; }

    public string? Devicename { get; set; }

    public DateOnly? Dateofcommissioning { get; set; }

    public string? Serialnumber { get; set; }

    public string? Inventorynumber { get; set; }

    public int? ModelId { get; set; }

    public int? OfficeId { get; set; }

    public bool? Exception { get; set; }

    public string? Note { get; set; }

    public virtual ICollection<DeviceWorkplace> DeviceWorkplaces { get; set; } = new List<DeviceWorkplace>();

    public virtual Devicetype? Devicetype { get; set; }

    public virtual Model? Model { get; set; }

    public virtual Office? Office { get; set; }

    public virtual ICollection<Devicepart> Deviceparts { get; set; } = new List<Devicepart>();
}
