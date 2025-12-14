namespace Ev.Reservation.Application.Services;

public interface IStationDirectoryClient
{
    Task<bool> ExistsAsync(Guid stationId, CancellationToken cancellationToken = default);
}
