namespace Shared.DTOs.ScoringConfiguration;


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
    public List<InputVariableOptionDto> Options { get; set; } = new List<InputVariableOptionDto>();
}