namespace Ev.Pricing.Domain;

public class TimeOfUseRule
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TariffPlanId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public decimal Multiplier { get; set; } = 1m;

    public void Validate()
    {
        if (Multiplier <= 0)
            throw new InvalidOperationException("Multiplier must be greater than zero.");
        if (StartTime >= EndTime)
            throw new InvalidOperationException("StartTime must be before EndTime.");
    }
}
