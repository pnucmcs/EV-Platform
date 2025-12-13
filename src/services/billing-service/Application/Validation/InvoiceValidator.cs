using Ev.Billing.Application.Requests;
using FluentValidation;

namespace Ev.Billing.Application.Validation;

public class CreateInvoiceValidator : AbstractValidator<CreateInvoiceRequest>
{
    public CreateInvoiceValidator()
    {
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Currency).NotEmpty().Length(3, 3);
    }
}
