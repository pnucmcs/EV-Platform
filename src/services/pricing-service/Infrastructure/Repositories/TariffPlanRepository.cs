using Ev.Pricing.Domain;
using Ev.Pricing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ev.Pricing.Infrastructure.Repositories;

public class TariffPlanRepository : ITariffPlanRepository
{
    private readonly PricingDbContext _db;

    public TariffPlanRepository(PricingDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(TariffPlan plan, CancellationToken cancellationToken)
    {
        await _db.TariffPlans.AddAsync(plan, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(TariffPlan plan, CancellationToken cancellationToken)
    {
        _db.TariffPlans.Remove(plan);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public Task<TariffPlan?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return _db.TariffPlans
            .Include(x => x.TimeOfUseRules)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<TariffPlan?> GetForStationAsync(Guid stationId, DateTime asOfUtc, CancellationToken cancellationToken)
    {
        return _db.TariffPlans
            .Include(x => x.TimeOfUseRules)
            .Where(x => x.IsActive && x.StationId == stationId && x.ValidFromUtc <= asOfUtc && (!x.ValidToUtc.HasValue || x.ValidToUtc >= asOfUtc))
            .OrderByDescending(x => x.ValidFromUtc)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TariffPlan>> ListAsync(CancellationToken cancellationToken)
    {
        return await _db.TariffPlans.Include(x => x.TimeOfUseRules).OrderByDescending(x => x.ValidFromUtc).ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(TariffPlan plan, CancellationToken cancellationToken)
    {
        _db.TariffPlans.Update(plan);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
