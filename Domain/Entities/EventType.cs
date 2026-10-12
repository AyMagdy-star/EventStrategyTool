using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class EventType
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int SortOrder { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
