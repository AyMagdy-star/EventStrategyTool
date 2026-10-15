namespace Shared.DTOs.ScoringConfiguration;
public class ScoringRulesDto
{
   
    public List<EvaluationFactorDto> Factors { get; set; } = new List<EvaluationFactorDto>();
    public List<BandingThresholdDto> Bands { get; set; } = new List<BandingThresholdDto>();
    public List<InputVariableDto> InputVariables { get; set; } = new List<InputVariableDto>();
}
