namespace Ev.Station.Application.Requests;

public sealed record CreateStationRequest(string Name, double Latitude, double Longitude);
