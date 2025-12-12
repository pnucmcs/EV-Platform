namespace Ev.Station.Application.Dtos;

public sealed record StationDto(
    Guid Id,
    string Name,
    double Latitude,
    double Longitude,
    string Status,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc,
    IReadOnlyCollection<ChargerDto> Chargers);
