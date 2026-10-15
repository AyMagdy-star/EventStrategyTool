using Core.ScoringConfiguration;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.ScoringConfiguration;

namespace EventStrategyToolAPI.Controllers;


[ApiController]
[Route("api/v1/scoring-configuration")]
public class ScoringConfigurationController : ControllerBase
{
    private readonly IScoringConfigurationService _scoringService;

    public ScoringConfigurationController(IScoringConfigurationService scoringService)
    {
        _scoringService = scoringService;
    }

   
    [HttpGet("factors")]
    public async Task<ActionResult<List<EvaluationFactorDto>>> GetFactors()
    {
        List<EvaluationFactorDto> factors = await _scoringService.GetFactorsWithKpisAsync();
        return Ok(factors);
    }

   
    [HttpPut("factors/weights")]
    public async Task<IActionResult> SaveFactorWeights([FromBody] List<WeightUpdateDto> weights)
    {
        List<string> errors = await _scoringService.SaveFactorWeightsAsync(weights);
        return ToResponse(errors);
    }

  
    [HttpPut("factors/{factorId}/kpi-weights")]
    public async Task<IActionResult> SaveKpiWeights(string factorId, [FromBody] List<WeightUpdateDto> weights)
    {
        List<string> errors = await _scoringService.SaveKpiWeightsAsync(factorId, weights);
        return ToResponse(errors);
    }

   
    [HttpGet("kpis/{kpiId}/bands")]
    public async Task<ActionResult<List<BandingThresholdDto>>> GetBands(string kpiId)
    {
        List<BandingThresholdDto> bands = await _scoringService.GetBandsAsync(kpiId);
        return Ok(bands);
    }

   
    [HttpPut("kpis/{kpiId}/bands")]
    public async Task<IActionResult> SaveBands(string kpiId, [FromBody] List<BandingThresholdDto> bands)
    {
        List<string> errors = await _scoringService.SaveBandsAsync(kpiId, bands);
        return ToResponse(errors);
    }

   
    [HttpGet("input-variables")]
    public async Task<ActionResult<List<InputVariableDto>>> GetInputVariables()
    {
        List<InputVariableDto> variables = await _scoringService.GetInputVariablesAsync();
        return Ok(variables);
    }

    
    [HttpGet("input-variables/{inputVariableId}")]
    public async Task<ActionResult<InputVariableDto>> GetInputVariable(string inputVariableId)
    {
        InputVariableDto? variable = await _scoringService.GetInputVariableAsync(inputVariableId);
        if (variable == null)
        {
            return NotFound();
        }

        return Ok(variable);
    }

    
    [HttpPut("input-variables/{inputVariableId}/options")]
    public async Task<IActionResult> SaveOptions(string inputVariableId, [FromBody] List<InputVariableOptionDto> options)
    {
        List<string> errors = await _scoringService.SaveOptionsAsync(inputVariableId, options);
        return ToResponse(errors);
    }

   
    [HttpPut("governance-weights")]
    public async Task<IActionResult> SaveGovernanceWeights([FromBody] List<WeightUpdateDto> weights)
    {
        List<string> errors = await _scoringService.SaveGovernanceWeightsAsync(weights);
        return ToResponse(errors);
    }

   
    [HttpGet("objectives")]
    public async Task<ActionResult<List<ObjectiveDto>>> GetObjectives()
    {
        List<ObjectiveDto> objectives = await _scoringService.GetObjectivesAsync();
        return Ok(objectives);
    }

  
    [HttpGet("objectives/{objectiveId}")]
    public async Task<ActionResult<ObjectiveDto>> GetObjective(string objectiveId)
    {
        ObjectiveDto? objective = await _scoringService.GetObjectiveAsync(objectiveId);
        if (objective == null)
        {
            return NotFound();
        }

        return Ok(objective);
    }

  
    [HttpPut("objectives/{objectiveId}")]
    public async Task<IActionResult> SaveObjective(string objectiveId, [FromBody] ObjectiveDto objective)
    {
       
        objective.Id = objectiveId;

        List<string> errors = await _scoringService.SaveObjectiveAsync(objective);
        return ToResponse(errors);
    }

   
    [HttpPut("objectives/{objectiveId}/benchmarks")]
    public async Task<IActionResult> SaveBenchmarks(string objectiveId, [FromBody] List<ObjectiveBenchmarkDto> benchmarks)
    {
        List<string> errors = await _scoringService.SaveBenchmarksAsync(objectiveId, benchmarks);
        return ToResponse(errors);
    }

    [HttpGet("rules")]
    public async Task<ActionResult<ScoringRulesDto>> GetScoringRules()
    {
        ScoringRulesDto rules = await _scoringService.GetScoringRulesAsync();
        return Ok(rules);
    }

   
    private IActionResult ToResponse(List<string> errors)
    {
        if (errors.Count == 0)
        {
            return Ok();
        }

        return BadRequest(new { errors = errors });
    }
}
