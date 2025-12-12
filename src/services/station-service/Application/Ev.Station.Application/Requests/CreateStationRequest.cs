namespace Ev.Station.Application.Requests;

public sealed record CreateStationRequest(string Name, string Location, int TotalSpots);
