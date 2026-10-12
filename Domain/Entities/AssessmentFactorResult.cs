using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class AssessmentFactorResult
{
    public string Id { get; set; } = null!;

    public string AssessmentId { get; set; } = null!;

    public string EvaluationFactorId { get; set; } = null!;

    public decimal Score { get; set; }

    public decimal Weight { get; set; }

    public decimal WeightedScore { get; set; }

    public string? Comment { get; set; }

    public virtual Assessment Assessment { get; set; } = null!;

    public virtual EvaluationFactor EvaluationFactor { get; set; } = null!;
}
