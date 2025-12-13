using AutoMapper;
using Ev.Pricing.Application.Models;
using Ev.Pricing.Application.Requests;
using Ev.Pricing.Domain;

namespace Ev.Pricing.Application.Services;

public class TariffPlanService : ITariffPlanService
{
    private readonly ITariffPlanRepository _repository;
    private readonly IMapper _mapper;

    public TariffPlanService(ITariffPlanRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<TariffPlanDto>> ListAsync(CancellationToken cancellationToken)
    {
        var plans = await _repository.ListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<TariffPlanDto>>(plans);
    }

    public async Task<TariffPlanDto?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var plan = await _repository.GetAsync(id, cancellationToken);
        return plan is null ? null : _mapper.Map<TariffPlanDto>(plan);
    }

    public async Task<TariffPlanDto> CreateAsync(CreateTariffPlanRequest request, CancellationToken cancellationToken)
    {
        var plan = _mapper.Map<TariffPlan>(request);
        plan.Validate();
        await _repository.AddAsync(plan, cancellationToken);
        return _mapper.Map<TariffPlanDto>(plan);
    }

    public async Task<TariffPlanDto?> UpdateAsync(Guid id, UpdateTariffPlanRequest request, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetAsync(id, cancellationToken);
        if (existing is null) return null;

        _mapper.Map(request, existing);
        existing.Validate();
        await _repository.UpdateAsync(existing, cancellationToken);
        return _mapper.Map<TariffPlanDto>(existing);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var plan = await _repository.GetAsync(id, cancellationToken);
        if (plan is null) return false;
        await _repository.DeleteAsync(plan, cancellationToken);
        return true;
    }

    public async Task<EstimateResponse?> EstimateAsync(EstimateRequest request, CancellationToken cancellationToken)
    {
        TariffPlan? plan = null;
        if (request.StationId.HasValue)
        {
            plan = await _repository.GetForStationAsync(request.StationId.Value, request.StartTimeUtc, cancellationToken);
        }
        plan ??= (await _repository.ListAsync(cancellationToken))
            .Where(p => p.IsActive && p.ValidFromUtc <= request.StartTimeUtc && (!p.ValidToUtc.HasValue || p.ValidToUtc.Value >= request.EndTimeUtc))
            .OrderByDescending(p => p.ValidFromUtc)
            .FirstOrDefault();
        if (plan is null) return null;

        var durationMinutes = (decimal)(request.EndTimeUtc - request.StartTimeUtc).TotalMinutes;
        var energyCost = request.EstimatedKwh * plan.BaseRatePerKwh;
        var idleCost = plan.IdleFeePerMinute * durationMinutes;

        var windowRules = plan.TimeOfUseRules
            .Where(r => r.DayOfWeek == request.StartTimeUtc.DayOfWeek)
            .ToList();

        if (windowRules.Any())
        {
            var windowStart = request.StartTimeUtc.TimeOfDay;
            foreach (var rule in windowRules)
            {
                if (windowStart >= rule.StartTime && windowStart < rule.EndTime)
                {
                    energyCost *= rule.Multiplier;
                    idleCost *= rule.Multiplier;
                    break;
                }
            }
        }

        var total = Math.Round(energyCost + idleCost, 2);
        return new EstimateResponse(total, plan.Currency, new EstimateBreakdown(Math.Round(energyCost, 2), Math.Round(idleCost, 2), total, plan.Currency));
    }
}
