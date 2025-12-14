using Ev.Billing.Domain;
using Ev.Billing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ev.Billing.Infrastructure.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly BillingDbContext _db;

    public InvoiceRepository(BillingDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Invoice invoice, CancellationToken cancellationToken)
    {
        await _db.Invoices.AddAsync(invoice, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public Task<Invoice?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return _db.Invoices.Include(x => x.PaymentAttempts).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<Invoice?> GetBySessionAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        return _db.Invoices.Include(x => x.PaymentAttempts).FirstOrDefaultAsync(x => x.SessionId == sessionId, cancellationToken);
    }

    public async Task<IReadOnlyList<Invoice>> GetByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _db.Invoices.Include(x => x.PaymentAttempts).Where(x => x.UserId == userId).OrderByDescending(x => x.CreatedAtUtc).ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(Invoice invoice, CancellationToken cancellationToken)
    {
        _db.Invoices.Update(invoice);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
