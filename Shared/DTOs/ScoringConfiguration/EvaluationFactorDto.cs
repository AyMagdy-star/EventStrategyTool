namespace Shared.DTOs.ScoringConfiguration;

public class EvaluationFactorDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Weight { get; set; }
    public int SortOrder { get; set; }

    public List<KpiDto> Kpis { get; set; } = new List<KpiDto>();
}