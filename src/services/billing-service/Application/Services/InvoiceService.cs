using AutoMapper;
using Ev.Billing.Application.Models;
using Ev.Billing.Application.Requests;
using Ev.Billing.Domain;

namespace Ev.Billing.Application.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _repository;
    private readonly IMapper _mapper;

    public InvoiceService(IInvoiceRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<InvoiceDto?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var invoice = await _repository.GetAsync(id, cancellationToken);
        return invoice is null ? null : _mapper.Map<InvoiceDto>(invoice);
    }

    public async Task<IReadOnlyList<InvoiceDto>> GetByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var invoices = await _repository.GetByUserAsync(userId, cancellationToken);
        return _mapper.Map<IReadOnlyList<InvoiceDto>>(invoices);
    }

    public async Task<InvoiceDto> CreateAsync(CreateInvoiceRequest request, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetBySessionAsync(request.SessionId, cancellationToken);
        if (existing is not null) return _mapper.Map<InvoiceDto>(existing);

        var invoice = _mapper.Map<Invoice>(request);
        invoice.Validate();
        await _repository.AddAsync(invoice, cancellationToken);
        return _mapper.Map<InvoiceDto>(invoice);
    }

    public async Task<InvoiceDto?> MarkPaidAsync(Guid id, string provider, CancellationToken cancellationToken)
    {
        var invoice = await _repository.GetAsync(id, cancellationToken);
        if (invoice is null) return null;
        invoice.Status = InvoiceStatus.Paid;
        invoice.PaymentAttempts.Add(new PaymentAttempt
        {
            InvoiceId = invoice.Id,
            Provider = provider,
            Status = PaymentStatus.Captured,
            CreatedAtUtc = DateTime.UtcNow
        });
        await _repository.UpdateAsync(invoice, cancellationToken);
        return _mapper.Map<InvoiceDto>(invoice);
    }
}
