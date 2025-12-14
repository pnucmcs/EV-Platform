using Ev.Reservation.Application.Requests;
using Ev.Reservation.Domain;
using FluentValidation;

namespace Ev.Reservation.Application.Validation;

public sealed class UpdateChargingSessionRequestValidator : AbstractValidator<UpdateChargingSessionRequest>
{
    public UpdateChargingSessionRequestValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum()
            .Must(status => status is ChargingSessionStatus.Completed or ChargingSessionStatus.Failed or ChargingSessionStatus.Cancelled)
            .WithMessage("Status must be Completed, Failed, or Cancelled.");

        When(x => x.Status == ChargingSessionStatus.Completed, () =>
        {
            RuleFor(x => x.EndedAtUtc)
                .NotNull()
                .WithMessage("EndedAtUtc is required when completing a session.");
        });
    }
}
