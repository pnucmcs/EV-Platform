namespace Ev.Pricing.Domain;

public interface ITariffPlanRepository
{
    Task<TariffPlan?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<TariffPlan?> GetForStationAsync(Guid stationId, DateTime asOfUtc, CancellationToken cancellationToken);
    Task<IReadOnlyList<TariffPlan>> ListAsync(CancellationToken cancellationToken);
    Task AddAsync(TariffPlan plan, CancellationToken cancellationToken);
    Task UpdateAsync(TariffPlan plan, CancellationToken cancellationToken);
    Task DeleteAsync(TariffPlan plan, CancellationToken cancellationToken);
}
