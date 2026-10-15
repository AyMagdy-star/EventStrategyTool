namespace Shared.DTOs.ScoringConfiguration;

public class ObjectiveDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Explanation { get; set; }
    public string? Rationale { get; set; }
    public decimal EmployeeAttendancePct { get; set; }
    public int TicketsPerEmployee { get; set; }
    public decimal DirectInvitesPct { get; set; }
    public List<ObjectiveBenchmarkDto> Benchmarks { get; set; } = new List<ObjectiveBenchmarkDto>();
}