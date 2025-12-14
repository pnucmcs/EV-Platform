namespace Ev.Station.Api.Configuration;

public sealed class AppOptions
{
    public string Name { get; set; } = "station-service";
    public string Version { get; set; } = "1.0.0";
    public string Environment { get; set; } = "Development";
}
