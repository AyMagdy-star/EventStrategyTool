namespace Shared.DTOs.ScoringConfiguration;

// One question or field of the evaluation, with its answers (dropdown) or items (checklist)
public class InputVariableDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string InputType { get; set; } = string.Empty;
    public string? Unit { get; set; }
    public string? KpiId { get; set; }
    public bool IsGovernance { get; set; }
    public decimal? GovernanceWeight { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }

    // Filled by the service: 5 answers for a dropdown, the items for a checklist, empty for numbers
    public List<InputVariableOptionDto> Options { get; set; } = new List<InputVariableOptionDto>();
}