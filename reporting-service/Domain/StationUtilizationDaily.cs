namespace Ev.Reporting.Domain;

public class StationUtilizationDaily
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid StationId { get; set; }
    public DateOnly Date { get; set; }
    public int SessionsCount { get; set; }
    public double TotalKwh { get; set; }
    public decimal TotalRevenue { get; set; }
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public void Validate()
    {
        if (StationId == Guid.Empty) throw new InvalidOperationException("StationId is required.");
        if (Date == default) throw new InvalidOperationException("Date is required.");
        if (UpdatedAtUtc.Kind != DateTimeKind.Utc) throw new InvalidOperationException("UpdatedAtUtc must be UTC.");
    }
}
