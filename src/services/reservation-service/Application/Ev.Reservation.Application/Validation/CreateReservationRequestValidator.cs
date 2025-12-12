using Ev.Reservation.Application.Requests;
using FluentValidation;

namespace Ev.Reservation.Application.Validation;

public sealed class CreateReservationRequestValidator : AbstractValidator<CreateReservationRequest>
{
    public CreateReservationRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.StationId).NotEmpty();
        RuleFor(x => x.StartsAtUtc).LessThan(x => x.EndsAtUtc);
    }
}
