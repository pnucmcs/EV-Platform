namespace Ev.Billing.Api.Configuration;

public class ServiceEndpointsOptions
{
    public PricingServiceOptions PricingService { get; set; } = new();
}

public class PricingServiceOptions
{
    public string? BaseUrl { get; set; }
}
