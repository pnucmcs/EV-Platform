using Ev.Billing.Application.Models;
using Ev.Billing.Application.Requests;

namespace Ev.Billing.Application.Services;

public interface IInvoiceService
{
    Task<InvoiceDto?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<InvoiceDto>> GetByUserAsync(Guid userId, CancellationToken cancellationToken);
    Task<InvoiceDto> CreateAsync(CreateInvoiceRequest request, CancellationToken cancellationToken);
    Task<InvoiceDto?> MarkPaidAsync(Guid id, string provider, CancellationToken cancellationToken);
}
