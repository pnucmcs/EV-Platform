using Ev.AdminConfig.Application.Requests;
using FluentValidation;

namespace Ev.AdminConfig.Application.Validation;

public class MaintenanceWindowValidator : AbstractValidator<CreateMaintenanceWindowRequest>
{
    public MaintenanceWindowValidator()
    {
        RuleFor(x => x.StationId).NotEmpty();
        RuleFor(x => x.StartUtc).Must(t => t.Kind == DateTimeKind.Utc).WithMessage("StartUtc must be UTC.");
        RuleFor(x => x.EndUtc).Must(t => t.Kind == DateTimeKind.Utc).WithMessage("EndUtc must be UTC.")
            .GreaterThan(x => x.StartUtc);
        RuleFor(x => x.Reason).NotEmpty();
    }
}

public class StationOpsConfigValidator : AbstractValidator<StationOpsConfigRequest>
{
    public StationOpsConfigValidator()
    {
        RuleFor(x => x.StationId).NotEmpty();
    }
}
