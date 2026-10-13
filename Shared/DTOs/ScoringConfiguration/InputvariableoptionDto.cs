namespace Shared.DTOs.ScoringConfiguration;

// A dropdown answer (Score 1 to 5) or a checklist item (Score empty)
public class InputVariableOptionDto
{
    public string Id { get; set; } = string.Empty;
    public string InputVariableId { get; set; } = string.Empty;
    public int? Score { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsOther { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
}