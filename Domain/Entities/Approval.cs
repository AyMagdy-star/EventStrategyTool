using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Approval
{
    public string Id { get; set; } = null!;

    public string EventId { get; set; } = null!;

    public string AssessmentId { get; set; } = null!;

    public string ApproverUserId { get; set; } = null!;

    public string ApproverRoleId { get; set; } = null!;

    public string DecisionOutcomeId { get; set; } = null!;

    public string? DecisionSummary { get; set; }

    public string? OverrideReason { get; set; }

    public DateTime SignedAt { get; set; }

    public virtual Role ApproverRole { get; set; } = null!;

    public virtual User ApproverUser { get; set; } = null!;

    public virtual Assessment Assessment { get; set; } = null!;

    public virtual DecisionOutcome DecisionOutcome { get; set; } = null!;

    public virtual Event Event { get; set; } = null!;
}
