namespace Shared.DTOs.ScoringConfiguration;
public class ObjectiveBenchmarkDto
{
    public string Id { get; set; } = string.Empty;
    public string ObjectiveId { get; set; } = string.Empty;
    public string MetricName { get; set; } = string.Empty;
    public string? KpiId { get; set; }
    public string? InputVariableId { get; set; }
    public string DisplayText { get; set; } = string.Empty;   
    public decimal? MinValue { get; set; }                     
    public decimal? MaxValue { get; set; }
    public int SortOrder { get; set; }
}