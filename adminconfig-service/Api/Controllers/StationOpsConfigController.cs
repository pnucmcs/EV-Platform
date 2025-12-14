using Ev.AdminConfig.Application.Models;
using Ev.AdminConfig.Application.Requests;
using Ev.AdminConfig.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ev.AdminConfig.Api.Controllers;

[ApiController]
[Route("api/v1/stations/{stationId:guid}/ops-config")]
public class StationOpsConfigController : ControllerBase
{
    private readonly IAdminConfigService _service;

    public StationOpsConfigController(IAdminConfigService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(StationOpsConfigDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StationOpsConfigDto>> Get(Guid stationId, CancellationToken cancellationToken)
    {
        var config = await _service.GetOpsConfigAsync(stationId, cancellationToken);
        return config is null ? NotFound() : Ok(config);
    }

    [HttpPut]
    [ProducesResponseType(typeof(StationOpsConfigDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<StationOpsConfigDto>> Upsert(Guid stationId, [FromBody] StationOpsConfigRequest request, CancellationToken cancellationToken)
    {
        request.StationId = stationId;
        var saved = await _service.UpsertOpsConfigAsync(request, cancellationToken);
        return Ok(saved);
    }
}
