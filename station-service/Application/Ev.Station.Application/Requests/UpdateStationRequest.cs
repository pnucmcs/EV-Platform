using Ev.Station.Domain;

namespace Ev.Station.Application.Requests;

public sealed record UpdateStationRequest(
    string Name,
    double Latitude,
    double Longitude,
    StationStatus Status);
