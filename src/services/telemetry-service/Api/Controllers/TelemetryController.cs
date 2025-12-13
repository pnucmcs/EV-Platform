using Ev.Telemetry.Application.Models;
using Ev.Telemetry.Application.Requests;
using Ev.Telemetry.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ev.Telemetry.Api.Controllers;

[ApiController]
[Route("api/v1/telemetry")]
public class TelemetryController : ControllerBase
{
    private readonly ITelemetryService _service;

    public TelemetryController(ITelemetryService service)
    {
        _service = service;
    }

    [HttpPost("readings")]
    public async Task<IActionResult> Ingest([FromBody] BulkTelemetryRequest request, CancellationToken cancellationToken)
    {
        await _service.IngestAsync(request.Readings, cancellationToken);
        return Accepted();
    }

    [HttpGet("stations/{stationId:guid}/latest")]
    public async Task<ActionResult<TelemetryReadingDto>> GetLatest(Guid stationId, CancellationToken cancellationToken)
    {
        var reading = await _service.GetLatestForStationAsync(stationId, cancellationToken);
        return reading is null ? NotFound() : Ok(reading);
    }
}
