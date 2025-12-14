namespace Ev.Reservation.Api.Configuration;

public sealed class AppOptions
{
    public string Name { get; set; } = "reservation-service";
    public string Version { get; set; } = "1.0.0";
    public string Environment { get; set; } = "Development";
}
