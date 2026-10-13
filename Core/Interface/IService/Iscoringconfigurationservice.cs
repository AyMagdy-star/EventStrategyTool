using Shared.DTOs.ScoringConfiguration;

namespace Core.ScoringConfiguration;

public interface IScoringConfigurationService
{
  
    Task<List<EvaluationFactorDto>> GetFactorsWithKpisAsync();
    Task<List<string>> SaveFactorWeightsAsync(List<WeightUpdateDto> weights);
    Task<List<string>> SaveKpiWeightsAsync(string factorId, List<WeightUpdateDto> weights);
    Task<List<BandingThresholdDto>> GetBandsAsync(string kpiId);
    Task<List<string>> SaveBandsAsync(string kpiId, List<BandingThresholdDto> bands);
    Task<List<InputVariableDto>> GetInputVariablesAsync();
    Task<InputVariableDto?> GetInputVariableAsync(string inputVariableId);
    Task<List<string>> SaveOptionsAsync(string inputVariableId, List<InputVariableOptionDto> options);
    Task<List<string>> SaveGovernanceWeightsAsync(List<WeightUpdateDto> weights);
}