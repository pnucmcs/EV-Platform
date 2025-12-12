using Ev.Station.Application.Requests;
using FluentValidation;

namespace Ev.Station.Application.Validation;

public sealed class CreateStationRequestValidator : AbstractValidator<CreateStationRequest>
{
    public CreateStationRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Location).NotEmpty().MaximumLength(500);
        RuleFor(x => x.TotalSpots).GreaterThan(0).LessThanOrEqualTo(500);
    }
}
