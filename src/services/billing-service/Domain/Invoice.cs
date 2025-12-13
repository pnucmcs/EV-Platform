namespace Ev.Billing.Domain;

public class Invoice
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SessionId { get; set; }
    public Guid UserId { get; set; }
    public Guid StationId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Pending;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public List<PaymentAttempt> PaymentAttempts { get; set; } = new();

    public void Validate()
    {
        if (Amount < 0) throw new InvalidOperationException("Amount must be non-negative.");
        if (string.IsNullOrWhiteSpace(Currency)) throw new InvalidOperationException("Currency is required.");
        if (SessionId == Guid.Empty) throw new InvalidOperationException("SessionId is required.");
        if (StationId == Guid.Empty) throw new InvalidOperationException("StationId is required.");
    }
}
