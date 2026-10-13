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
}