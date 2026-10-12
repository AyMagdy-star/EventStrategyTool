using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class BandingThreshold
{
    public string Id { get; set; } = null!;

    public string KpiId { get; set; } = null!;

    public byte Score { get; set; }

    public decimal? MinValue { get; set; }

    public decimal? MaxValue { get; set; }

    public virtual Kpi Kpi { get; set; } = null!;
}
