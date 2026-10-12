using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class InputVariable
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string InputType { get; set; } = null!;

    public string? Unit { get; set; }

    public string? KpiId { get; set; }

    public bool IsGovernance { get; set; }

    public decimal? GovernanceWeight { get; set; }

    public int SortOrder { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<AssessmentInputAnswer> AssessmentInputAnswers { get; set; } = new List<AssessmentInputAnswer>();

    public virtual ICollection<InputVariableOption> InputVariableOptions { get; set; } = new List<InputVariableOption>();

    public virtual Kpi? Kpi { get; set; }

    public virtual ICollection<ObjectiveBenchMark> ObjectiveBenchMarks { get; set; } = new List<ObjectiveBenchMark>();
}
