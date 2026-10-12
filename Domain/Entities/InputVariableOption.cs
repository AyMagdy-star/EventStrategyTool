using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class InputVariableOption
{
    public string Id { get; set; } = null!;

    public string InputVariableId { get; set; } = null!;

    public byte? Score { get; set; }

    public string Description { get; set; } = null!;

    public bool IsOther { get; set; }

    public int SortOrder { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<AssessmentInputAnswer> AssessmentInputAnswers { get; set; } = new List<AssessmentInputAnswer>();

    public virtual InputVariable InputVariable { get; set; } = null!;
}
