using Ev.Station.Application.Requests;
using FluentValidation;

namespace Ev.Station.Application.Validation;

public sealed class UpdateChargerRequestValidator : AbstractValidator<UpdateChargerRequest>
{
    public UpdateChargerRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ConnectorType).IsInEnum().NotEqual(Ev.Station.Domain.ConnectorType.Unknown);
        RuleFor(x => x.Status).IsInEnum().NotEqual(Ev.Station.Domain.ChargerStatus.Unknown);
    }
}
