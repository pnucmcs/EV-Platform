using Ev.AdminConfig.Application.Models;
using Ev.AdminConfig.Application.Requests;
using Ev.AdminConfig.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ev.AdminConfig.Api.Controllers;

[ApiController]
[Route("api/v1/maintenance-windows")]
public class MaintenanceWindowsController : ControllerBase
{
    private readonly IAdminConfigService _service;

    public MaintenanceWindowsController(IAdminConfigService service)
    {
        _service = service;
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(MaintenanceWindowDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MaintenanceWindowDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var window = await _service.GetMaintenanceWindowAsync(id, cancellationToken);
        return window is null ? NotFound() : Ok(window);
    }

    [HttpGet("/api/v1/stations/{stationId:guid}/maintenance-windows")]
    [ProducesResponseType(typeof(IEnumerable<MaintenanceWindowDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MaintenanceWindowDto>>> GetForStation(Guid stationId, CancellationToken cancellationToken)
    {
        var windows = await _service.GetMaintenanceWindowsAsync(stationId, cancellationToken);
        return Ok(windows);
    }

    [HttpPost]
    [ProducesResponseType(typeof(MaintenanceWindowDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<MaintenanceWindowDto>> Create([FromBody] CreateMaintenanceWindowRequest request, CancellationToken cancellationToken)
    {
        var created = await _service.CreateMaintenanceWindowAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(MaintenanceWindowDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MaintenanceWindowDto>> Update(Guid id, [FromBody] UpdateMaintenanceWindowRequest request, CancellationToken cancellationToken)
    {
        var updated = await _service.UpdateMaintenanceWindowAsync(id, request, cancellationToken);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _service.DeleteMaintenanceWindowAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
