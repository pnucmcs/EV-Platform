namespace Ev.Station.Infrastructure;

public sealed class StationMongoSettings
{
    public string ConnectionString { get; set; } = "mongodb://localhost:27017";
    public string Database { get; set; } = "ev_platform";
    public string Collection { get; set; } = "stations";
}
