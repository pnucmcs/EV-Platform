using Ev.Reservation.Application.Requests;
using Ev.Reservation.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ev.Reservation.Api.Controllers;

[ApiController]
[Route("api/v1/sessions")]
public class ChargingSessionsController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ChargingSessionsController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpGet("by-reservation/{reservationId:guid}")]
    public async Task<IActionResult> GetByReservation(Guid reservationId, CancellationToken cancellationToken)
    {
        var sessions = await _reservationService.GetSessionsAsync(reservationId, cancellationToken);
        return Ok(sessions);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var session = await _reservationService.GetSessionByIdAsync(id, cancellationToken);
        return session is null ? NotFound() : Ok(session);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateChargingSessionRequest request, CancellationToken cancellationToken)
    {
        var session = await _reservationService.CreateSessionAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = session.Id }, session);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateChargingSessionRequest request, CancellationToken cancellationToken)
    {
        var session = await _reservationService.UpdateSessionAsync(id, request, cancellationToken);
        return session is null ? NotFound() : Ok(session);
    }
}
