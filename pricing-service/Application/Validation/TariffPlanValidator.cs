using Ev.Pricing.Application.Requests;
using FluentValidation;

namespace Ev.Pricing.Application.Validation;

public class TariffPlanValidator : AbstractValidator<CreateTariffPlanRequest>
{
    public TariffPlanValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Currency).NotEmpty().Length(3, 3);
        RuleFor(x => x.BaseRatePerKwh).GreaterThanOrEqualTo(0);
        RuleFor(x => x.IdleFeePerMinute).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ValidFromUtc).NotEmpty();
        RuleFor(x => x.ValidToUtc).Must((req, to) => !to.HasValue || to > req.ValidFromUtc).WithMessage("ValidToUtc must be after ValidFromUtc.");
        RuleForEach(x => x.TimeOfUseRules).SetValidator(new TimeOfUseRuleValidator());
    }
}

public class TimeOfUseRuleValidator : AbstractValidator<TimeOfUseRuleRequest>
{
    public TimeOfUseRuleValidator()
    {
        RuleFor(x => x.Multiplier).GreaterThan(0);
        RuleFor(x => x.StartTime).LessThan(x => x.EndTime);
    }
}

public class EstimateRequestValidator : AbstractValidator<EstimateRequest>
{
    public EstimateRequestValidator()
    {
        RuleFor(x => x.EstimatedKwh).GreaterThanOrEqualTo(0);
        RuleFor(x => x.EndTimeUtc).GreaterThan(x => x.StartTimeUtc);
    }
}
