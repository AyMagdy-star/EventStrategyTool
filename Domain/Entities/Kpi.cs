using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Kpi
{
    public string Id { get; set; } = null!;

    public string EvaluationFactorId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public decimal Weight { get; set; }

    public string ScoringMethod { get; set; } = null!;

    public string? Unit { get; set; }

    public string? AlignedDepartmentId { get; set; }

    public int SortOrder { get; set; }

    public virtual Department? AlignedDepartment { get; set; }

    public virtual ICollection<AssessmentKpiValue> AssessmentKpiValues { get; set; } = new List<AssessmentKpiValue>();

    public virtual ICollection<BandingThreshold> BandingThresholds { get; set; } = new List<BandingThreshold>();

    public virtual ICollection<DepartmentAlignment> DepartmentAlignments { get; set; } = new List<DepartmentAlignment>();

    public virtual EvaluationFactor EvaluationFactor { get; set; } = null!;

    public virtual ICollection<InputVariable> InputVariables { get; set; } = new List<InputVariable>();

    public virtual ICollection<ObjectiveBenchMark> ObjectiveBenchMarks { get; set; } = new List<ObjectiveBenchMark>();
}
