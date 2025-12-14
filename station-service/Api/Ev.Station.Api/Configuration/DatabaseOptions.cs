namespace Ev.Station.Api.Configuration;

public sealed class DatabaseOptions
{
    public string Provider { get; set; } = "Postgres";
    public string? ConnectionString { get; set; }
}
