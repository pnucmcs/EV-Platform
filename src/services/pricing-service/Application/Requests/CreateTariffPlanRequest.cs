using System.ComponentModel.DataAnnotations;

namespace Ev.Pricing.Application.Requests;

public class CreateTariffPlanRequest
{
    public Guid? StationId { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string Currency { get; set; } = "USD";
    public decimal BaseRatePerKwh { get; set; }
    public decimal IdleFeePerMinute { get; set; }
    public DateTime ValidFromUtc { get; set; }
    public DateTime? ValidToUtc { get; set; }
    public bool IsActive { get; set; } = true;
    public List<TimeOfUseRuleRequest> TimeOfUseRules { get; set; } = new();
}

public class TimeOfUseRuleRequest
{
    [Range(0, 6)]
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public decimal Multiplier { get; set; } = 1m;
}
