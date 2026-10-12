using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class AssessmentKpiValue
{
    public string Id { get; set; } = null!;

    public string AssessmentId { get; set; } = null!;

    public string KpiId { get; set; } = null!;

    public decimal? InputValue { get; set; }

    public decimal? Score { get; set; }

    public decimal? WeightedScore { get; set; }

    public string? Comment { get; set; }

    public virtual Assessment Assessment { get; set; } = null!;

    public virtual Kpi Kpi { get; set; } = null!;
}
