namespace Ev.Pricing.Application.Models;

public record TariffPlanDto(
    Guid Id,
    Guid? StationId,
    string Name,
    string Currency,
    decimal BaseRatePerKwh,
    decimal IdleFeePerMinute,
    DateTime ValidFromUtc,
    DateTime? ValidToUtc,
    bool IsActive,
    IReadOnlyCollection<TimeOfUseRuleDto> TimeOfUseRules);

public record TimeOfUseRuleDto(
    Guid Id,
    DayOfWeek DayOfWeek,
    TimeSpan StartTime,
    TimeSpan EndTime,
    decimal Multiplier);
