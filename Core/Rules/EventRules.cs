using Shared.DTOs.Events;

namespace Core.Rules;

/// <summary>Pure validation and constants for events. No database access, so it is easy to unit test.</summary>
internal static class EventRules
{
    // TODO: confirm these against the seed data (P0.5a). EventDepartment.DepartmentRole is nvarchar(20).
    public const string DraftStatusName = "Draft";
    public const string LeadingDepartment = "Leading";
    public const string SupportingDepartment = "Supporting";

    /// <summary>Returns every problem found, or an empty list when the event is valid.</summary>
    public static List<string> Validate(SaveEventDto d)
    {
        var errors = new List<string>();

        Required(errors, d.Name, 150, "Name");
        Required(errors, d.EventTypeId, 10, "Event type");
        Required(errors, d.PrimaryObjectiveId, 10, "Primary objective");
        if (d.EventDate == default) errors.Add("Event date is required.");

        MaxLength(errors, d.Location, 100, "Location");
        MaxLength(errors, d.Venue, 200, "Venue");
        MaxLength(errors, d.TargetAudience, 200, "Target audience");
        MaxLength(errors, d.AccessLevels, 500, "Access levels");
        MaxLength(errors, d.TicketTypes, 500, "Ticket types");
        MaxLength(errors, d.SpecialExperiences, 500, "Special experiences");

        if (d.NumberOfSponsors < 0) errors.Add("Number of sponsors cannot be negative.");

        if (!string.IsNullOrWhiteSpace(d.SecondaryObjectiveId) && d.SecondaryObjectiveId == d.PrimaryObjectiveId)
            errors.Add("Secondary objective must be different from the primary objective.");
        if (!string.IsNullOrWhiteSpace(d.SecondaryIcpId) && d.SecondaryIcpId == d.PrimaryIcpId)
            errors.Add("Secondary ICP must be different from the primary ICP.");

        ValidateDepartments(errors, d.Departments);

        foreach (var r in d.Responsibilities)
        {
            Required(errors, r.ResponsibilityType, 30, "Responsibility type");
            if (string.IsNullOrWhiteSpace(r.UserId) && string.IsNullOrWhiteSpace(r.PersonName))
                errors.Add($"Responsibility '{r.ResponsibilityType}' needs a user or a person name.");
            MaxLength(errors, r.PersonName, 100, "Person name");
        }

        foreach (var r in d.RisksAndUpsides)
        {
            Required(errors, r.ItemType, 10, "Risk/upside type");
            Required(errors, r.Description, 500, "Risk/upside description");
        }

        return errors;
    }

    private static void ValidateDepartments(List<string> errors, IReadOnlyList<EventDepartmentDto> departments)
    {
        var leading = departments.Count(x => x.DepartmentRole == LeadingDepartment);
        if (leading != 1) errors.Add("Exactly one leading department is required.");

        if (departments.Any(x => x.DepartmentRole is not (LeadingDepartment or SupportingDepartment)))
            errors.Add($"Department role must be '{LeadingDepartment}' or '{SupportingDepartment}'.");

        if (departments.GroupBy(x => x.DepartmentId).Any(g => g.Count() > 1))
            errors.Add("A department can only be listed once.");
    }

    private static void Required(List<string> errors, string? value, int max, string label)
    {
        if (string.IsNullOrWhiteSpace(value)) errors.Add($"{label} is required.");
        else MaxLength(errors, value, max, label);
    }

    private static void MaxLength(List<string> errors, string? value, int max, string label)
    {
        if (value is not null && value.Trim().Length > max)
            errors.Add($"{label} cannot be longer than {max} characters.");
    }
}
