using AutoMapper;
using Ev.Billing.Application.Models;
using Ev.Billing.Application.Requests;
using Ev.Billing.Domain;

namespace Ev.Billing.Application.Mapping;

public class BillingProfile : Profile
{
    public BillingProfile()
    {
        CreateMap<CreateInvoiceRequest, Invoice>();
        CreateMap<Invoice, InvoiceDto>();
        CreateMap<PaymentAttempt, PaymentAttemptDto>();
    }
}
