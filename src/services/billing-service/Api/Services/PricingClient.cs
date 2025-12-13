using System.Net.Http.Json;
using System.Text.Json;
using Ev.Billing.Application.Requests;

namespace Ev.Billing.Api.Services;

public interface IPricingClient
{
    Task<decimal?> EstimateAsync(Guid stationId, DateTime start, DateTime end, double? energyKwh, CancellationToken cancellationToken);
}

public class PricingClient : IPricingClient
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public PricingClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<decimal?> EstimateAsync(Guid stationId, DateTime start, DateTime end, double? energyKwh, CancellationToken cancellationToken)
    {
        var request = new
        {
            stationId,
            startTimeUtc = start,
            endTimeUtc = end,
            estimatedKwh = energyKwh ?? 0
        };

        try
        {
            var response = await _client.PostAsJsonAsync("/api/v1/pricing/estimate", request, _jsonOptions, cancellationToken);
            if (!response.IsSuccessStatusCode) return null;
            var body = await response.Content.ReadFromJsonAsync<PricingEstimateResponse>(_jsonOptions, cancellationToken);
            return body?.Total;
        }
        catch
        {
            return null;
        }
    }

    private record PricingEstimateResponse(decimal Total, string Currency, EstimateBreakdown Breakdown);
    private record EstimateBreakdown(decimal EnergyCost, decimal IdleCost, decimal Total, string Currency);
}
