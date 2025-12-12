using Ev.Station.Application.Requests;
using FluentValidation;

namespace Ev.Station.Application.Validation;

public sealed class UpdateStationRequestValidator : AbstractValidator<UpdateStationRequest>
{
    public UpdateStationRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90);
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180);
        RuleFor(x => x.Status).IsInEnum();
    }
}
