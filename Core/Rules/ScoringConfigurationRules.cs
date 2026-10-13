using Shared.DTOs.ScoringConfiguration;

namespace Core.ScoringConfiguration;

// Plain rules with no database access, so they are easy to read and reuse
public static class ScoringConfigurationRules
{
    // Small margin so 0.333333 x 3 still counts as 100%
    private const decimal AllowedDifference = 0.0001m;

    // ---------- Weights ----------

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

    // ---------- Score bands ----------

    // A KPI needs exactly 5 bands (scores 1 to 5) that follow each other with no gaps:
    // score 1 has no minimum, score 5 has no maximum,
    // and each band ends exactly where the next one starts.
    public static List<string> CheckBands(List<BandingThresholdDto> bands)
    {
        List<string> errors = new List<string>();

        if (bands.Count != 5)
        {
            errors.Add("Bands: a KPI needs exactly 5 bands, one for each score from 1 to 5.");
            return errors;
        }

        // Put the bands in score order 1..5, and make sure each score appears once
        BandingThresholdDto?[] bandsByScore = new BandingThresholdDto?[6];
        foreach (BandingThresholdDto band in bands)
        {
            if (band.Score < 1 || band.Score > 5)
            {
                errors.Add($"Bands: score {band.Score} is not allowed, scores go from 1 to 5.");
                continue;
            }

            if (bandsByScore[band.Score] != null)
            {
                errors.Add($"Bands: score {band.Score} appears more than once.");
                continue;
            }

            bandsByScore[band.Score] = band;
        }

        if (errors.Count > 0)
        {
            return errors;
        }

        for (int score = 1; score <= 5; score++)
        {
            BandingThresholdDto band = bandsByScore[score]!;

            // Score 1 starts from nothing, every other score needs a minimum
            if (score > 1 && band.MinValue == null)
            {
                errors.Add($"Bands: score {score} needs a minimum value.");
            }

            // Score 5 goes to infinity, every other score needs a maximum
            if (score < 5 && band.MaxValue == null)
            {
                errors.Add($"Bands: score {score} needs a maximum value.");
            }

            if (band.MinValue != null && band.MaxValue != null && band.MinValue >= band.MaxValue)
            {
                errors.Add($"Bands: for score {score}, the minimum must be smaller than the maximum.");
            }

            // No gap and no overlap: this band ends where the next one starts
            if (score < 5)
            {
                BandingThresholdDto nextBand = bandsByScore[score + 1]!;
                if (band.MaxValue != null && nextBand.MinValue != null && band.MaxValue != nextBand.MinValue)
                {
                    errors.Add($"Bands: score {score} ends at {band.MaxValue} but score {score + 1} starts at {nextBand.MinValue}. They must be the same value.");
                }
            }
        }

        return errors;
    }
}