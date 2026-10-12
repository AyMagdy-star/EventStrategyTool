using Shared.DTOs.ScoringConfiguration;

namespace Core.ScoringConfiguration;

public static class ScoringConfigurationRules
{
  
    private const decimal AllowedDifference = 0.0001m;

    public static List<string> CheckWeights(List<WeightUpdateDto> weights, string groupName)
    {
        List<string> errors = new List<string>();

        if (weights.Count == 0)
        {
            errors.Add($"{groupName}: there are no weights to save.");
            return errors;
        }

        decimal total = 0m;
        foreach (WeightUpdateDto item in weights)
        {
            if (item.Weight < 0m || item.Weight > 1m)
            {
                errors.Add($"{groupName}: each weight must be between 0% and 100%.");
            }

            total += item.Weight;
        }

        decimal difference = Math.Abs(total - 1m);
        if (difference > AllowedDifference)
        {
            decimal totalPercent = Math.Round(total * 100m, 2);
            errors.Add($"{groupName}: weights add up to {totalPercent}%, they must add up to 100%.");
        }

        return errors;
    }
}