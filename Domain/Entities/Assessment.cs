using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Assessment
{
    public string Id { get; set; } = null!;

    public string EventId { get; set; } = null!;

    public decimal? RoiPure { get; set; }

    public decimal? RoiInclMarketing { get; set; }

    public decimal? ScalabilityScore { get; set; }

    public decimal? TotalScore { get; set; }

    public string? RecommendedOutcomeId { get; set; }

    public string? SubmittedByUserId { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public bool IsLocked { get; set; }

    public string Phase { get; set; } = null!;

    public virtual ICollection<Approval> Approvals { get; set; } = new List<Approval>();

    public virtual ICollection<AssessmentFactorResult> AssessmentFactorResults { get; set; } = new List<AssessmentFactorResult>();

    public virtual ICollection<AssessmentInputAnswer> AssessmentInputAnswers { get; set; } = new List<AssessmentInputAnswer>();

    public virtual ICollection<AssessmentKpiValue> AssessmentKpiValues { get; set; } = new List<AssessmentKpiValue>();

    public virtual ICollection<DepartmentAlignment> DepartmentAlignments { get; set; } = new List<DepartmentAlignment>();

    public virtual Event Event { get; set; } = null!;

    public virtual DecisionOutcome? RecommendedOutcome { get; set; }

    public virtual User? SubmittedByUser { get; set; }
}
