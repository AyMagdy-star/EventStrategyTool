using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class PnlLine
{
    public string Id { get; set; } = null!;

    public string EventId { get; set; } = null!;

    public string PnlLineTypeId { get; set; } = null!;

    public decimal ExpectedAmount { get; set; }

    public decimal? ActualAmount { get; set; }

    public decimal? Difference { get; set; }

    public decimal? RateUsed { get; set; }

    public bool IsManualOverride { get; set; }

    public string? Note { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual PnlLineType PnlLineType { get; set; } = null!;
}
