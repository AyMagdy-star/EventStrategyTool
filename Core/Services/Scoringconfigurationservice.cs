using Core.Interface;
using Core.Interface.IRepository;
using Core.Mapper;
using Domain.Entities;
using Shared.DTOs.ScoringConfiguration;

namespace Core.ScoringConfiguration;

public class ScoringConfigurationService : IScoringConfigurationService
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public ScoringConfigurationService(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task<List<EvaluationFactorDto>> GetFactorsWithKpisAsync()
    {
        await using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IRepository<EvaluationFactor> factorRepository = unitOfWork.Repository<EvaluationFactor>();
        IRepository<Kpi> kpiRepository = unitOfWork.Repository<Kpi>();

        List<EvaluationFactor> factors = await factorRepository.ListAsync();
        List<Kpi> kpis = await kpiRepository.ListAsync();

        factors.Sort((first, second) => first.SortOrder.CompareTo(second.SortOrder));
        kpis.Sort((first, second) => first.SortOrder.CompareTo(second.SortOrder));

        List<EvaluationFactorDto> result = new List<EvaluationFactorDto>();

        foreach (EvaluationFactor factor in factors)
        {
            EvaluationFactorDto factorDto = GenericMapper.Map<EvaluationFactor, EvaluationFactorDto>(factor);

            foreach (Kpi kpi in kpis)
            {
                if (kpi.EvaluationFactorId == factor.Id)
                {
                    factorDto.Kpis.Add(GenericMapper.Map<Kpi, KpiDto>(kpi));
                }
            }

            result.Add(factorDto);
        }

        return result;
    }

    public async Task<List<string>> SaveFactorWeightsAsync(List<WeightUpdateDto> weights)
    {
        List<string> errors = ScoringConfigurationRules.CheckWeights(weights, "Factors");
        if (errors.Count > 0)
        {
            return errors;
        }

        await using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();
        IRepository<EvaluationFactor> factorRepository = unitOfWork.Repository<EvaluationFactor>();

        List<EvaluationFactor> factors = await factorRepository.ListAsync();

       
        if (factors.Count != weights.Count)
        {
            errors.Add("Factors: send the weight of every factor together.");
            return errors;
        }

        foreach (WeightUpdateDto item in weights)
        {
            EvaluationFactor? factor = FindFactor(factors, item.Id);
            if (factor == null)
            {
                errors.Add($"Factors: {item.Id} was not found.");
                continue;
            }

            factor.Weight = item.Weight;
            factorRepository.Update(factor);
        }

        if (errors.Count > 0)
        {
            return errors;
        }

        await unitOfWork.SaveChangesAsync();
        return errors;
    }

    public async Task<List<string>> SaveKpiWeightsAsync(string factorId, List<WeightUpdateDto> weights)
    {
        List<string> errors = ScoringConfigurationRules.CheckWeights(weights, "KPIs");
        if (errors.Count > 0)
        {
            return errors;
        }

        await using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();
        IRepository<Kpi> kpiRepository = unitOfWork.Repository<Kpi>();

      
        List<Kpi> factorKpis = await kpiRepository.ListAsync(k => k.EvaluationFactorId == factorId);

        if (factorKpis.Count != weights.Count)
        {
            errors.Add("KPIs: send the weight of every KPI of this factor together.");
            return errors;
        }

        foreach (WeightUpdateDto item in weights)
        {
            Kpi? kpi = FindKpi(factorKpis, item.Id);
            if (kpi == null)
            {
                errors.Add($"KPIs: {item.Id} does not belong to this factor.");
                continue;
            }

            kpi.Weight = item.Weight;
            kpiRepository.Update(kpi);
        }

        if (errors.Count > 0)
        {
            return errors;
        }

        await unitOfWork.SaveChangesAsync();
        return errors;
    }

   
    public async Task<List<BandingThresholdDto>> GetBandsAsync(string kpiId)
    {
        await using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();
        IRepository<BandingThreshold> bandRepository = unitOfWork.Repository<BandingThreshold>();

        List<BandingThreshold> bands = await bandRepository.ListAsync(b => b.KpiId == kpiId);
        bands.Sort((first, second) => Convert.ToInt32(first.Score).CompareTo(Convert.ToInt32(second.Score)));

        List<BandingThresholdDto> result = new List<BandingThresholdDto>();

        foreach (BandingThreshold band in bands)
        {
            BandingThresholdDto bandDto = GenericMapper.Map<BandingThreshold, BandingThresholdDto>(band);

            
            bandDto.Score = Convert.ToInt32(band.Score);

            result.Add(bandDto);
        }

        return result;
    }

    public async Task<List<string>> SaveBandsAsync(string kpiId, List<BandingThresholdDto> bands)
    {
        List<string> errors = ScoringConfigurationRules.CheckBands(bands);
        if (errors.Count > 0)
        {
            return errors;
        }

        await using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();
        IRepository<Kpi> kpiRepository = unitOfWork.Repository<Kpi>();
        IRepository<BandingThreshold> bandRepository = unitOfWork.Repository<BandingThreshold>();

        Kpi? kpi = await kpiRepository.GetByIdAsync(kpiId);
        if (kpi == null)
        {
            errors.Add($"Bands: KPI {kpiId} was not found.");
            return errors;
        }

       
        List<BandingThreshold> existingBands = await bandRepository.ListAsync(b => b.KpiId == kpiId);

        foreach (BandingThresholdDto item in bands)
        {
            BandingThreshold? band = FindBandByScore(existingBands, item.Score);
            if (band == null)
            {
                errors.Add($"Bands: score {item.Score} does not exist for this KPI. Check the seed data.");
                continue;
            }

            band.MinValue = item.MinValue;
            band.MaxValue = item.MaxValue;
            bandRepository.Update(band);
        }

        if (errors.Count > 0)
        {
            return errors;
        }

        await unitOfWork.SaveChangesAsync();
        return errors;
    }

   
    public async Task<List<InputVariableDto>> GetInputVariablesAsync()
    {
        await using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IRepository<InputVariable> variableRepository = unitOfWork.Repository<InputVariable>();
        IRepository<InputVariableOption> optionRepository = unitOfWork.Repository<InputVariableOption>();

        List<InputVariable> variables = await variableRepository.ListAsync();
        List<InputVariableOption> options = await optionRepository.ListAsync();

        variables.Sort((first, second) => first.SortOrder.CompareTo(second.SortOrder));
        options.Sort((first, second) => first.SortOrder.CompareTo(second.SortOrder));

        List<InputVariableDto> result = new List<InputVariableDto>();

        foreach (InputVariable variable in variables)
        {
            InputVariableDto variableDto = GenericMapper.Map<InputVariable, InputVariableDto>(variable);

            foreach (InputVariableOption option in options)
            {
                if (option.InputVariableId == variable.Id)
                {
                    variableDto.Options.Add(ToOptionDto(option));
                }
            }

            result.Add(variableDto);
        }

        return result;
    }

    public async Task<InputVariableDto?> GetInputVariableAsync(string inputVariableId)
    {
        await using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IRepository<InputVariable> variableRepository = unitOfWork.Repository<InputVariable>();
        IRepository<InputVariableOption> optionRepository = unitOfWork.Repository<InputVariableOption>();

        InputVariable? variable = await variableRepository.GetByIdAsync(inputVariableId);
        if (variable == null)
        {
            return null;
        }

        List<InputVariableOption> options = await optionRepository.ListAsync(o => o.InputVariableId == inputVariableId);
        options.Sort((first, second) => first.SortOrder.CompareTo(second.SortOrder));

        InputVariableDto variableDto = GenericMapper.Map<InputVariable, InputVariableDto>(variable);

        foreach (InputVariableOption option in options)
        {
            variableDto.Options.Add(ToOptionDto(option));
        }

        return variableDto;
    }

    public async Task<List<string>> SaveOptionsAsync(string inputVariableId, List<InputVariableOptionDto> options)
    {
        List<string> errors = new List<string>();

        await using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();
        IRepository<InputVariable> variableRepository = unitOfWork.Repository<InputVariable>();
        IRepository<InputVariableOption> optionRepository = unitOfWork.Repository<InputVariableOption>();

        InputVariable? variable = await variableRepository.GetByIdAsync(inputVariableId);
        if (variable == null)
        {
            errors.Add($"Options: input {inputVariableId} was not found.");
            return errors;
        }

      
        errors = ScoringConfigurationRules.CheckOptions(variable.InputType, options);
        if (errors.Count > 0)
        {
            return errors;
        }

       
        List<InputVariableOption> existingOptions = await optionRepository.ListAsync(o => o.InputVariableId == inputVariableId);

        if (existingOptions.Count != options.Count)
        {
            errors.Add("Options: send every answer or item of this input together.");
            return errors;
        }

        foreach (InputVariableOptionDto item in options)
        {
            InputVariableOption? option = FindOption(existingOptions, item.Id);
            if (option == null)
            {
                errors.Add($"Options: {item.Id} does not belong to this input.");
                continue;
            }

           
            option.Description = item.Description.Trim();
            option.SortOrder = item.SortOrder;
            option.IsActive = item.IsActive;
            optionRepository.Update(option);
        }

        if (errors.Count > 0)
        {
            return errors;
        }

        await unitOfWork.SaveChangesAsync();
        return errors;
    }

    public async Task<List<string>> SaveGovernanceWeightsAsync(List<WeightUpdateDto> weights)
    {
        List<string> errors = ScoringConfigurationRules.CheckWeights(weights, "Governance");
        if (errors.Count > 0)
        {
            return errors;
        }

        await using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();
        IRepository<InputVariable> variableRepository = unitOfWork.Repository<InputVariable>();

        List<InputVariable> governanceVariables = await variableRepository.ListAsync(v => v.IsGovernance);

       
        if (governanceVariables.Count != weights.Count)
        {
            errors.Add("Governance: send the weight of every governance question together.");
            return errors;
        }

        foreach (WeightUpdateDto item in weights)
        {
            InputVariable? variable = FindVariable(governanceVariables, item.Id);
            if (variable == null)
            {
                errors.Add($"Governance: {item.Id} is not a governance question.");
                continue;
            }

            variable.GovernanceWeight = item.Weight;
            variableRepository.Update(variable);
        }

        if (errors.Count > 0)
        {
            return errors;
        }

        await unitOfWork.SaveChangesAsync();
        return errors;
    }

  

    public async Task<List<ObjectiveDto>> GetObjectivesAsync()
    {
        await using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();
        IRepository<Objective> objectiveRepository = unitOfWork.Repository<Objective>();

        List<Objective> objectives = await objectiveRepository.ListAsync();

        
        objectives.Sort((first, second) => string.CompareOrdinal(first.Id, second.Id));

        List<ObjectiveDto> result = GenericMapper.MapList<Objective, ObjectiveDto>(objectives);
        return result;
    }

    public async Task<ObjectiveDto?> GetObjectiveAsync(string objectiveId)
    {
        await using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IRepository<Objective> objectiveRepository = unitOfWork.Repository<Objective>();
        IRepository<ObjectiveBenchMark> benchmarkRepository = unitOfWork.Repository<ObjectiveBenchMark>();

        Objective? objective = await objectiveRepository.GetByIdAsync(objectiveId);
        if (objective == null)
        {
            return null;
        }

        List<ObjectiveBenchMark> benchmarks = await benchmarkRepository.ListAsync(b => b.ObjectiveId == objectiveId);
        benchmarks.Sort((first, second) => first.SortOrder.CompareTo(second.SortOrder));

        ObjectiveDto objectiveDto = GenericMapper.Map<Objective, ObjectiveDto>(objective);
        objectiveDto.Benchmarks = GenericMapper.MapList<ObjectiveBenchMark, ObjectiveBenchmarkDto>(benchmarks);

        return objectiveDto;
    }

    public async Task<List<string>> SaveObjectiveAsync(ObjectiveDto objective)
    {
        List<string> errors = ScoringConfigurationRules.CheckObjective(objective);
        if (errors.Count > 0)
        {
            return errors;
        }

        await using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();
        IRepository<Objective> objectiveRepository = unitOfWork.Repository<Objective>();

        Objective? existing = await objectiveRepository.GetByIdAsync(objective.Id);
        if (existing == null)
        {
            errors.Add($"Objective: {objective.Id} was not found.");
            return errors;
        }

       
        string newName = objective.Name.Trim();
        bool nameTaken = await objectiveRepository.AnyAsync(o => o.Name == newName && o.Id != objective.Id);
        if (nameTaken)
        {
            errors.Add($"Objective: another objective is already called '{newName}'.");
            return errors;
        }

       
        existing.Name = newName;
        existing.Explanation = objective.Explanation;
        existing.Rationale = objective.Rationale;
        existing.EmployeeAttendancePct = objective.EmployeeAttendancePct;
        existing.TicketsPerEmployee = objective.TicketsPerEmployee;
        existing.DirectInvitesPct = objective.DirectInvitesPct;
        objectiveRepository.Update(existing);

        await unitOfWork.SaveChangesAsync();
        return errors;
    }

    public async Task<List<string>> SaveBenchmarksAsync(string objectiveId, List<ObjectiveBenchmarkDto> benchmarks)
    {
        List<string> errors = ScoringConfigurationRules.CheckBenchmarks(benchmarks);
        if (errors.Count > 0)
        {
            return errors;
        }

        await using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();
        IRepository<ObjectiveBenchMark> benchmarkRepository = unitOfWork.Repository<ObjectiveBenchMark>();

       
        List<ObjectiveBenchMark> existingBenchmarks = await benchmarkRepository.ListAsync(b => b.ObjectiveId == objectiveId);

        foreach (ObjectiveBenchmarkDto item in benchmarks)
        {
            ObjectiveBenchMark? benchmark = FindBenchmark(existingBenchmarks, item.Id);
            if (benchmark == null)
            {
                errors.Add($"Benchmarks: {item.Id} does not belong to this objective.");
                continue;
            }

         
            benchmark.DisplayText = item.DisplayText.Trim();
            benchmark.MinValue = item.MinValue;
            benchmark.MaxValue = item.MaxValue;
            benchmarkRepository.Update(benchmark);
        }

        if (errors.Count > 0)
        {
            return errors;
        }

        await unitOfWork.SaveChangesAsync();
        return errors;
    }

 

    public async Task<ScoringRulesDto> GetScoringRulesAsync()
    {
       
        await using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        List<EvaluationFactor> factors = await unitOfWork.Repository<EvaluationFactor>().ListAsync();
        List<Kpi> kpis = await unitOfWork.Repository<Kpi>().ListAsync();
        List<BandingThreshold> bands = await unitOfWork.Repository<BandingThreshold>().ListAsync();
        List<InputVariable> variables = await unitOfWork.Repository<InputVariable>().ListAsync(v => v.IsActive);
        List<InputVariableOption> options = await unitOfWork.Repository<InputVariableOption>().ListAsync(o => o.IsActive);

        factors.Sort((first, second) => first.SortOrder.CompareTo(second.SortOrder));
        kpis.Sort((first, second) => first.SortOrder.CompareTo(second.SortOrder));
        variables.Sort((first, second) => first.SortOrder.CompareTo(second.SortOrder));
        options.Sort((first, second) => first.SortOrder.CompareTo(second.SortOrder));

        ScoringRulesDto rules = new ScoringRulesDto();

       
        foreach (EvaluationFactor factor in factors)
        {
            EvaluationFactorDto factorDto = GenericMapper.Map<EvaluationFactor, EvaluationFactorDto>(factor);

            foreach (Kpi kpi in kpis)
            {
                if (kpi.EvaluationFactorId == factor.Id)
                {
                    factorDto.Kpis.Add(GenericMapper.Map<Kpi, KpiDto>(kpi));
                }
            }

            rules.Factors.Add(factorDto);
        }

        
        bands.Sort((first, second) =>
        {
            int byKpi = string.CompareOrdinal(first.KpiId, second.KpiId);
            if (byKpi != 0)
            {
                return byKpi;
            }

            return Convert.ToInt32(first.Score).CompareTo(Convert.ToInt32(second.Score));
        });

        foreach (BandingThreshold band in bands)
        {
            BandingThresholdDto bandDto = GenericMapper.Map<BandingThreshold, BandingThresholdDto>(band);
            bandDto.Score = Convert.ToInt32(band.Score);
            rules.Bands.Add(bandDto);
        }

        
        foreach (InputVariable variable in variables)
        {
            InputVariableDto variableDto = GenericMapper.Map<InputVariable, InputVariableDto>(variable);

            foreach (InputVariableOption option in options)
            {
                if (option.InputVariableId == variable.Id)
                {
                    variableDto.Options.Add(ToOptionDto(option));
                }
            }

            rules.InputVariables.Add(variableDto);
        }

        return rules;
    }

    private static InputVariableOptionDto ToOptionDto(InputVariableOption option)
    {
        InputVariableOptionDto optionDto = GenericMapper.Map<InputVariableOption, InputVariableOptionDto>(option);

        if (option.Score != null)
        {
            optionDto.Score = option.Score.Value;
        }
        else
        {
            optionDto.Score = null;
        }

        return optionDto;
    }

    private static EvaluationFactor? FindFactor(List<EvaluationFactor> factors, string id)
    {
        foreach (EvaluationFactor factor in factors)
        {
            if (factor.Id == id)
            {
                return factor;
            }
        }

        return null;
    }

    private static Kpi? FindKpi(List<Kpi> kpis, string id)
    {
        foreach (Kpi kpi in kpis)
        {
            if (kpi.Id == id)
            {
                return kpi;
            }
        }

        return null;
    }

    private static BandingThreshold? FindBandByScore(List<BandingThreshold> bands, int score)
    {
        foreach (BandingThreshold band in bands)
        {
            if (Convert.ToInt32(band.Score) == score)
            {
                return band;
            }
        }

        return null;
    }

    private static InputVariable? FindVariable(List<InputVariable> variables, string id)
    {
        foreach (InputVariable variable in variables)
        {
            if (variable.Id == id)
            {
                return variable;
            }
        }

        return null;
    }

    private static InputVariableOption? FindOption(List<InputVariableOption> options, string id)
    {
        foreach (InputVariableOption option in options)
        {
            if (option.Id == id)
            {
                return option;
            }
        }

        return null;
    }

    private static ObjectiveBenchMark? FindBenchmark(List<ObjectiveBenchMark> benchmarks, string id)
    {
        foreach (ObjectiveBenchMark benchmark in benchmarks)
        {
            if (benchmark.Id == id)
            {
                return benchmark;
            }
        }

        return null;
    }
}
