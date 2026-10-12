using Shared.DTOs.ScoringConfiguration;

namespace Core.ScoringConfiguration;

public interface IScoringConfigurationService
{
    
    Task<List<EvaluationFactorDto>> GetFactorsWithKpisAsync();
    Task<List<string>> SaveFactorWeightsAsync(List<WeightUpdateDto> weights);
    Task<List<string>> SaveKpiWeightsAsync(string factorId, List<WeightUpdateDto> weights);
}