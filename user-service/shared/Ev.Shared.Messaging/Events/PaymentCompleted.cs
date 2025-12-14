namespace Ev.Shared.Messaging.Events;

public sealed record PaymentCompleted(Guid InvoiceId, Guid ReservationId, decimal Amount, string Currency, DateTime PaidAtUtc);
