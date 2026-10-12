using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class EventResponsibility
{
    public string Id { get; set; } = null!;

    public string EventId { get; set; } = null!;

    public string ResponsibilityType { get; set; } = null!;

    public string? UserId { get; set; }

    public string? PersonName { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual User? User { get; set; }
}
