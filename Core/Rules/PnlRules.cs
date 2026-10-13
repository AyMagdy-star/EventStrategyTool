using Shared.DTOs.Pnl;

namespace Core.Rules;

internal static class PnlRules
{
    public static List<string> Validate(PnlInputDto i)
    {
        var errors = new List<string>();

        if (i.PayingTiers is null || i.NonPayingTiers is null)
        {
            errors.Add("Ticket tiers are required (they can be empty).");
            return errors;
        }

        CheckTiers(errors, i.PayingTiers, "Paying tier");
        CheckTiers(errors, i.NonPayingTiers, "Invite/employee tier");

        NonNegative(errors, i.SponsorRevenue, "Sponsor revenue");
        NonNegative(errors, i.ProductionCost, "Production cost");
        NonNegative(errors, i.ArtistCost, "Artist cost");
        NonNegative(errors, i.ValidationFee, "Validation fee");
        NonNegative(errors, i.InvitationFee, "Invitation fee");
        NonNegative(errors, i.PoliceServiceCost, "Police service cost");
        NonNegative(errors, i.PermissionsCost, "Permissions cost");
        NonNegative(errors, i.DigitalMarketingCost, "Digital marketing cost");
        NonNegative(errors, i.MarketingBudget, "Marketing budget");
        NonNegative(errors, i.OtherTaxPerPayingTicket, "Other tax per ticket");

        Rate(errors, i.VatRate, "VAT rate");
        Rate(errors, i.EntertainmentTaxRate, "Entertainment tax rate");
        Rate(errors, i.CommissionRate, "Commission rate");

        return errors;
    }

    private static void CheckTiers(List<string> errors, IReadOnlyList<TicketLineDto> tiers, string label)
    {
        for (var n = 0; n < tiers.Count; n++)
        {
            if (tiers[n].UnitPrice < 0) errors.Add($"{label} {n + 1}: price cannot be negative.");
            if (tiers[n].Quantity < 0) errors.Add($"{label} {n + 1}: quantity cannot be negative.");
        }
    }

    private static void NonNegative(List<string> errors, decimal value, string label)
    {
        if (value < 0) errors.Add($"{label} cannot be negative.");
    }

    private static void Rate(List<string> errors, decimal value, string label)
    {
        if (value is < 0 or > 1) errors.Add($"{label} must be between 0% and 100%.");
    }
}
