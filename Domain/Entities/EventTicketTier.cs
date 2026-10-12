using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class EventTicketTier
{
    public string Id { get; set; } = null!;

    public string EventId { get; set; } = null!;

    public string TicketTierKindId { get; set; } = null!;

    public int TierNumber { get; set; }

    public string? TierName { get; set; }

    public decimal UnitPrice { get; set; }

    public int ExpectedQuantity { get; set; }

    public int? ActualQuantity { get; set; }

    public decimal? ExpectedTotal { get; set; }

    public decimal? ActualTotal { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual TicketTierKind TicketTierKind { get; set; } = null!;
}
