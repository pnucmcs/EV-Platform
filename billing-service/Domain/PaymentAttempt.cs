namespace Ev.Billing.Domain;

public class PaymentAttempt
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid InvoiceId { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Initiated;
    public string Provider { get; set; } = "stub";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public string? LastError { get; set; }
}
