namespace Ev.Telemetry.Application.Models;

public record TelemetryReadingDto(
    Guid Id,
    string DeviceId,
    Guid StationId,
    DateTime TimestampUtc,
    decimal Voltage,
    decimal PowerKw,
    decimal EnergyKwhDelta,
    decimal TemperatureC,
    string Status);
