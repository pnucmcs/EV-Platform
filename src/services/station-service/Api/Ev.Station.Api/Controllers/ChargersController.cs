using Ev.Station.Application.Requests;
using Ev.Station.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ev.Station.Api.Controllers;

[ApiController]
[Route("api/v1/stations/{stationId:guid}/chargers")]
public class ChargersController : ControllerBase
{
    private readonly IStationService _stationService;

    public ChargersController(IStationService stationService)
    {
        _stationService = stationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(Guid stationId, CancellationToken cancellationToken)
    {
        var chargers = await _stationService.GetChargersAsync(stationId, cancellationToken);
        return chargers is null ? NotFound() : Ok(chargers);
    }

    [HttpGet("{chargerId:guid}")]
    public async Task<IActionResult> GetById(Guid stationId, Guid chargerId, CancellationToken cancellationToken)
    {
        var charger = await _stationService.GetChargerByIdAsync(stationId, chargerId, cancellationToken);
        return charger is null ? NotFound() : Ok(charger);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid stationId, [FromBody] CreateChargerRequest request, CancellationToken cancellationToken)
    {
        var charger = await _stationService.CreateChargerAsync(stationId, request, cancellationToken);
        if (charger is null)
        {
            return NotFound();
        }

        return CreatedAtAction(nameof(GetById), new { stationId, chargerId = charger.Id }, charger);
    }

    [HttpPut("{chargerId:guid}")]
    public async Task<IActionResult> Update(Guid stationId, Guid chargerId, [FromBody] UpdateChargerRequest request, CancellationToken cancellationToken)
    {
        var charger = await _stationService.UpdateChargerAsync(stationId, chargerId, request, cancellationToken);
        return charger is null ? NotFound() : Ok(charger);
    }

    [HttpDelete("{chargerId:guid}")]
    public async Task<IActionResult> Delete(Guid stationId, Guid chargerId, CancellationToken cancellationToken)
    {
        var removed = await _stationService.DeleteChargerAsync(stationId, chargerId, cancellationToken);
        return removed ? NoContent() : NotFound();
    }
}
