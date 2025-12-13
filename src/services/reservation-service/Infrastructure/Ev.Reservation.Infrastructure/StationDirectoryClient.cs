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
        HttpResponseMessage response;
        try
        {
            response = await _httpClient.GetAsync($"/api/v1/stations/{stationId}", cancellationToken);
        }
        catch (TaskCanceledException)
        {
            throw new HttpRequestException("StationService request timed out.", null, System.Net.HttpStatusCode.ServiceUnavailable);
        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException("StationService unreachable.", ex, System.Net.HttpStatusCode.ServiceUnavailable);
        }
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
