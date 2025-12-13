namespace Ev.Billing.Domain;

public interface IInvoiceRepository
{
    Task<Invoice?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<Invoice?> GetBySessionAsync(Guid sessionId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Invoice>> GetByUserAsync(Guid userId, CancellationToken cancellationToken);
    Task AddAsync(Invoice invoice, CancellationToken cancellationToken);
    Task UpdateAsync(Invoice invoice, CancellationToken cancellationToken);
}
