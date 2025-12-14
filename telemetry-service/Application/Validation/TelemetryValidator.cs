using Ev.Telemetry.Application.Requests;
using FluentValidation;

namespace Ev.Telemetry.Application.Validation;

public class TelemetryReadingValidator : AbstractValidator<TelemetryReadingRequest>
{
    public TelemetryReadingValidator()
    {
        RuleFor(x => x.DeviceId).NotEmpty();
        RuleFor(x => x.StationId).NotEmpty();
        RuleFor(x => x.TimestampUtc).Must(t => t.Kind == DateTimeKind.Utc).WithMessage("TimestampUtc must be UTC.");
    }
}

public class BulkTelemetryValidator : AbstractValidator<BulkTelemetryRequest>
{
    public BulkTelemetryValidator()
    {
        RuleFor(x => x.Readings).NotEmpty();
        RuleForEach(x => x.Readings).SetValidator(new TelemetryReadingValidator());
    }
}
