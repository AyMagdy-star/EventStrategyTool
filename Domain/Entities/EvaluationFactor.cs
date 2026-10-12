using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class EvaluationFactor
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public decimal Weight { get; set; }

    public int SortOrder { get; set; }

    public virtual ICollection<AssessmentFactorResult> AssessmentFactorResults { get; set; } = new List<AssessmentFactorResult>();

    public virtual ICollection<Kpi> Kpis { get; set; } = new List<Kpi>();
}
