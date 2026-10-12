using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class ComparisonMetric
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Unit { get; set; } = null!;

    public bool HigherIsBetter { get; set; }

    public decimal TolerancePct { get; set; }

    public int SortOrder { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<EventComparison> EventComparisons { get; set; } = new List<EventComparison>();
}
