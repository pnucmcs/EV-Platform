namespace Ev.Station.Application.Dtos;

public sealed record ChargerDto(
    Guid Id,
    string Name,
    string ConnectorType,
    string Status,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);
