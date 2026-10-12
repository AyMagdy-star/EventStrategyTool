using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class EventComparison
{
    public string Id { get; set; } = null!;

    public string EventId { get; set; } = null!;

    public string ComparisonMetricId { get; set; } = null!;

    public decimal? ExpectedValue { get; set; }

    public decimal? ActualValue { get; set; }

    public decimal? Difference { get; set; }

    public decimal? DifferencePct { get; set; }

    public string? Comment { get; set; }

    public string? ExpectedEnteredByUserId { get; set; }

    public string? ActualEnteredByUserId { get; set; }

    public DateTime? ActualEnteredAt { get; set; }

    public virtual User? ActualEnteredByUser { get; set; }

    public virtual ComparisonMetric ComparisonMetric { get; set; } = null!;

    public virtual Event Event { get; set; } = null!;

    public virtual User? ExpectedEnteredByUser { get; set; }
}
