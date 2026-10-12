using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Icp
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Event> EventPrimaryIcps { get; set; } = new List<Event>();

    public virtual ICollection<Event> EventSecondaryIcps { get; set; } = new List<Event>();
}
