namespace Ev.Station.Application.Dtos;

public sealed record StationDto(Guid Id, string Name, string Location, int TotalSpots, string Status);
