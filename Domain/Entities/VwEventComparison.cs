using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class VwEventComparison
{
    public string EventId { get; set; } = null!;

    public string EventName { get; set; } = null!;

    public int SortOrder { get; set; }

    public string Metric { get; set; } = null!;

    public string Unit { get; set; } = null!;

    public decimal? ExpectedValue { get; set; }

    public decimal? ActualValue { get; set; }

    public decimal? Difference { get; set; }

    public decimal? DifferencePct { get; set; }

    public string Result { get; set; } = null!;

    public string? Comment { get; set; }
}
