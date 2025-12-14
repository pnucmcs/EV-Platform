using System.Collections.Generic;

namespace Ev.Pricing.Domain;

public class TariffPlan
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? StationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Currency { get; set; } = "USD";
    public decimal BaseRatePerKwh { get; set; }
    public decimal IdleFeePerMinute { get; set; }
    public DateTime ValidFromUtc { get; set; }
    public DateTime? ValidToUtc { get; set; }
    public bool IsActive { get; set; } = true;
    public List<TimeOfUseRule> TimeOfUseRules { get; set; } = new();

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new InvalidOperationException("Name is required.");
        if (string.IsNullOrWhiteSpace(Currency))
            throw new InvalidOperationException("Currency is required.");
        if (BaseRatePerKwh < 0)
            throw new InvalidOperationException("Base rate must be non-negative.");
        if (IdleFeePerMinute < 0)
            throw new InvalidOperationException("Idle fee must be non-negative.");
        if (ValidToUtc.HasValue && ValidToUtc < ValidFromUtc)
            throw new InvalidOperationException("ValidToUtc must be after ValidFromUtc.");
        foreach (var rule in TimeOfUseRules)
        {
            rule.Validate();
        }
    }
}
