namespace Shared.DTOs.ScoringConfiguration;

// One of the 6 event goals, with its ticket rules and its recommended ranges
public class ObjectiveDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Explanation { get; set; }
    public string? Rationale { get; set; }

    // Ticket rules, as fractions: 0.10 = 10% of total attendees
    public decimal EmployeeAttendancePct { get; set; }
    public int TicketsPerEmployee { get; set; }
    public decimal DirectInvitesPct { get; set; }

    // Filled by the service when one objective is opened
    public List<ObjectiveBenchmarkDto> Benchmarks { get; set; } = new List<ObjectiveBenchmarkDto>();
}