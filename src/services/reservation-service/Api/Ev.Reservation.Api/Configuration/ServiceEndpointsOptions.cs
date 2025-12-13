namespace Ev.Reservation.Api.Configuration;

public sealed class ServiceEndpointsOptions
{
    public StationServiceOptions StationService { get; set; } = new();
}

public sealed class StationServiceOptions
{
    public string BaseUrl { get; set; } = "http://localhost:8080";
}
