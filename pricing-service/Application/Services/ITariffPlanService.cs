using Ev.Pricing.Application.Models;
using Ev.Pricing.Application.Requests;

namespace Ev.Pricing.Application.Services;

public interface ITariffPlanService
{
    Task<IReadOnlyList<TariffPlanDto>> ListAsync(CancellationToken cancellationToken);
    Task<TariffPlanDto?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<TariffPlanDto> CreateAsync(CreateTariffPlanRequest request, CancellationToken cancellationToken);
    Task<TariffPlanDto?> UpdateAsync(Guid id, UpdateTariffPlanRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<EstimateResponse?> EstimateAsync(EstimateRequest request, CancellationToken cancellationToken);
}
