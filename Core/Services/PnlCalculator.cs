using Core.Interface.IService;
using Core.Rules;
using Shared.DTOs.Pnl;
using Shared.Results;

namespace Core.Services;

/// <summary>
/// Implements the formulas of the workbook's "P&amp;L Statement" sheet, with the hard-coded 14%, 30% and 7.01
/// turned into inputs. Uses decimal throughout and does no rounding, like the workbook.
/// </summary>
internal sealed class PnlCalculator : IPnlCalculator
{
    public Result<PnlResultDto> Calculate(PnlInputDto i)
    {
        var errors = PnlRules.Validate(i);
        if (errors.Count > 0) return Error.Validation("pnl.invalid", string.Join(" ", errors));

        // Revenue
        var gross = i.PayingTiers.Sum(t => t.UnitPrice * t.Quantity);
        var totalRevenue = gross + i.SponsorRevenue;
        var inviteLoss = i.NonPayingTiers.Sum(t => t.UnitPrice * t.Quantity);
        var netRevenue = totalRevenue - inviteLoss;

        var payingCount = i.PayingTiers.Sum(t => t.Quantity);
        var nonPayingCount = i.NonPayingTiers.Sum(t => t.Quantity);

        // Tax. Ticket prices include VAT; entertainment tax is taken from the price net of VAT.
        // Workbook: VAT = gross / 1.14 * 0.14, entertainment = (gross - VAT) / 1.3 * 30%.
        var vat = gross / (1 + i.VatRate) * i.VatRate;
        var entertainment = (gross - vat) / (1 + i.EntertainmentTaxRate) * i.EntertainmentTaxRate;
        var otherTax = payingCount * i.OtherTaxPerPayingTicket;
        var tax = vat + entertainment + otherTax;

        // Costs
        var venue = i.ProductionCost + i.ArtistCost;
        var commission = gross * i.CommissionRate;   // ASSUMPTION: commission is charged on gross ticket revenue
        var ticketingFees = commission + i.ValidationFee + i.InvitationFee;
        var otherCosts = i.PoliceServiceCost + i.PermissionsCost + i.DigitalMarketingCost;
        var cost = tax + venue + ticketingFees + otherCosts;

        // Result
        var profit = netRevenue - cost;
        decimal? margin = netRevenue == 0 ? null : profit / netRevenue;
        var profitInclMarketing = profit + i.MarketingBudget;
        decimal? roi = cost == 0 ? null : profitInclMarketing / cost;

        return new PnlResultDto(
            payingCount, nonPayingCount,
            gross, i.SponsorRevenue, totalRevenue, inviteLoss, netRevenue,
            vat, entertainment, otherTax, tax,
            venue, commission, ticketingFees, otherCosts, cost,
            profit, margin, profitInclMarketing, roi);
    }
}
