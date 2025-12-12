using Ev.Reservation.Application.Services;
using System.Net;

namespace Ev.Reservation.Infrastructure;

public sealed class StationDirectoryClient : IStationDirectoryClient
{
    private readonly HttpClient _httpClient;

    public StationDirectoryClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> ExistsAsync(Guid stationId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"/api/v1/stations/{stationId}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        if (response.IsSuccessStatusCode)
        {
            return true;
        }

        // Treat other statuses as failure to validate
        response.EnsureSuccessStatusCode();
        return false;
    }
}
