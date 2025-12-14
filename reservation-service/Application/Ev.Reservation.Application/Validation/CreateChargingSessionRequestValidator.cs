using Ev.Reservation.Application.Requests;
using FluentValidation;

namespace Ev.Reservation.Application.Validation;

public sealed class CreateChargingSessionRequestValidator : AbstractValidator<CreateChargingSessionRequest>
{
    public CreateChargingSessionRequestValidator()
    {
        RuleFor(x => x.ReservationId).NotEmpty();
        RuleFor(x => x.StationId).NotEmpty();
        RuleFor(x => x.StartedAtUtc).LessThanOrEqualTo(DateTime.UtcNow.AddMinutes(5));
    }
}
