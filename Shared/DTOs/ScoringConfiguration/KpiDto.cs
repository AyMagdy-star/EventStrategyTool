namespace Shared.DTOs.ScoringConfiguration;

public class KpiDto
{
    public string Id { get; set; } = string.Empty;
    public string EvaluationFactorId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Weight { get; set; }
    public string ScoringMethod { get; set; } = string.Empty;
    public string? Unit { get; set; }
    public int SortOrder { get; set; }
}