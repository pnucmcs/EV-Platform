using Ev.Station.Application.Requests;
using Ev.Station.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ev.Station.Api.Controllers;

[ApiController]
[Route("api/v1/stations")]
public class StationsController : ControllerBase
{
    private readonly IStationService _stationService;

    public StationsController(IStationService stationService)
    {
        _stationService = stationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var stations = await _stationService.GetAllAsync(cancellationToken);
        return Ok(stations);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var station = await _stationService.GetByIdAsync(id, cancellationToken);
        return station is null ? NotFound() : Ok(station);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStationRequest request, CancellationToken cancellationToken)
    {
        var station = await _stationService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = station.Id }, station);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStationRequest request, CancellationToken cancellationToken)
    {
        var station = await _stationService.UpdateAsync(id, request, cancellationToken);
        return station is null ? NotFound() : Ok(station);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _stationService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
