using Ev.Billing.Domain;

namespace Ev.Billing.Application.Models;

public record PaymentAttemptDto(Guid Id, PaymentStatus Status, string Provider, DateTime CreatedAtUtc, string? LastError);

public record InvoiceDto(
    Guid Id,
    Guid SessionId,
    Guid UserId,
    Guid StationId,
    decimal Amount,
    string Currency,
    InvoiceStatus Status,
    DateTime CreatedAtUtc,
    IReadOnlyCollection<PaymentAttemptDto> PaymentAttempts);
