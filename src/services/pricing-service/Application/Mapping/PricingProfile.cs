using AutoMapper;
using Ev.Pricing.Application.Models;
using Ev.Pricing.Application.Requests;
using Ev.Pricing.Domain;

namespace Ev.Pricing.Application.Mapping;

public class PricingProfile : Profile
{
    public PricingProfile()
    {
        CreateMap<CreateTariffPlanRequest, TariffPlan>();
        CreateMap<TimeOfUseRuleRequest, TimeOfUseRule>();
        CreateMap<TariffPlan, TariffPlanDto>();
        CreateMap<TimeOfUseRule, TimeOfUseRuleDto>();
    }
}
