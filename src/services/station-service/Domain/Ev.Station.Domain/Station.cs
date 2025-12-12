namespace Ev.Station.Domain;

public enum StationStatus
{
    Unknown = 0,
    Online = 1,
    Offline = 2,
    Maintenance = 3
}

public sealed class Station
{
    public Guid Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int TotalSpots { get; set; }
    public StationStatus Status { get; set; } = StationStatus.Unknown;
}
