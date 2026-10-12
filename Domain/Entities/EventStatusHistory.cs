using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class EventStatusHistory
{
    public string Id { get; set; } = null!;

    public string EventId { get; set; } = null!;

    public string? FromStatusId { get; set; }

    public string ToStatusId { get; set; } = null!;

    public string ChangedByUserId { get; set; } = null!;

    public DateTime ChangedAt { get; set; }

    public string? Comment { get; set; }

    public virtual User ChangedByUser { get; set; } = null!;

    public virtual Event Event { get; set; } = null!;

    public virtual EventStatus? FromStatus { get; set; }

    public virtual EventStatus ToStatus { get; set; } = null!;
}
