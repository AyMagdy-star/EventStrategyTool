namespace Shared.DTOs.ScoringConfiguration;

// One row of a KPI's score table, for example: Leads from 0.10 to 0.15 gives score 3
public class BandingThresholdDto
{
    public string Id { get; set; } = string.Empty;
    public string KpiId { get; set; } = string.Empty;
    public int Score { get; set; }

    // Percentages are fractions: 0.10 = 10%
    // MinValue is included, MaxValue is not. Empty means "no limit" (score 1 has no minimum, score 5 has no maximum)
    public decimal? MinValue { get; set; }
    public decimal? MaxValue { get; set; }
}