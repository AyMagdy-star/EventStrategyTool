using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class AssessmentInputAnswer
{
    public string Id { get; set; } = null!;

    public string AssessmentId { get; set; } = null!;

    public string InputVariableId { get; set; } = null!;

    public string? InputVariableOptionId { get; set; }

    public decimal? NumberValue { get; set; }

    public string? OtherText { get; set; }

    public virtual Assessment Assessment { get; set; } = null!;

    public virtual InputVariable InputVariable { get; set; } = null!;

    public virtual InputVariableOption? InputVariableOption { get; set; }
}
