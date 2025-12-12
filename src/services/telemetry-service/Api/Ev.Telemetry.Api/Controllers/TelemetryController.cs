using Microsoft.AspNetCore.Mvc;

namespace Ev.Telemetry.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TelemetryController : ControllerBase
{
    private static readonly List<TelemetryDto> Events = new();

    [HttpGet]
    public IActionResult GetAll() => Ok(Events);

    [HttpPost]
    public IActionResult Publish([FromBody] PublishTelemetryRequest request)
    {
        var evt = new TelemetryDto(Guid.NewGuid(), request.StationId, request.Status, DateTime.UtcNow);
        Events.Add(evt);
        return Ok(evt);
    }
}

public record PublishTelemetryRequest(Guid StationId, string Status);
public record TelemetryDto(Guid Id, Guid StationId, string Status, DateTime OccurredAtUtc);
