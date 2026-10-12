using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class ObjectiveBenchMark
{
    public string Id { get; set; } = null!;

    public string ObjectiveId { get; set; } = null!;

    public string MetricName { get; set; } = null!;

    public string? KpiId { get; set; }

    public string? InputVariableId { get; set; }

    public string DisplayText { get; set; } = null!;

    public decimal? MinValue { get; set; }

    public decimal? MaxValue { get; set; }

    public int SortOrder { get; set; }

    public virtual InputVariable? InputVariable { get; set; }

    public virtual Kpi? Kpi { get; set; }

    public virtual Objective Objective { get; set; } = null!;
}
