using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class TicketTierKind
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public bool IsPaying { get; set; }

    public int SortOrder { get; set; }

    public virtual ICollection<EventTicketTier> EventTicketTiers { get; set; } = new List<EventTicketTier>();
}
