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
}