using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class EventStatus
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int SortOrder { get; set; }

    public virtual ICollection<EventStatusHistory> EventStatusHistoryFromStatuses { get; set; } = new List<EventStatusHistory>();

    public virtual ICollection<EventStatusHistory> EventStatusHistoryToStatuses { get; set; } = new List<EventStatusHistory>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
