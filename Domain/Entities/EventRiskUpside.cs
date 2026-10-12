using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class EventRiskUpside
{
    public string Id { get; set; } = null!;

    public string EventId { get; set; } = null!;

    public string ItemType { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int SortOrder { get; set; }

    public virtual Event Event { get; set; } = null!;
}
