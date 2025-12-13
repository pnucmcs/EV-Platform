using AutoMapper;
using Ev.Pricing.Application.Models;
using Ev.Pricing.Application.Requests;
using Ev.Pricing.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ev.Pricing.Api.Controllers;

[ApiController]
[Route("api/v1/tariff-plans")]
public class TariffPlansController : ControllerBase
{
    private readonly ITariffPlanService _service;

    public TariffPlansController(ITariffPlanService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TariffPlanDto>>> List(CancellationToken cancellationToken)
    {
        var plans = await _service.ListAsync(cancellationToken);
        return Ok(plans);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TariffPlanDto>> Get(Guid id, CancellationToken cancellationToken)
    {
        var plan = await _service.GetAsync(id, cancellationToken);
        if (plan is null) return NotFound();
        return Ok(plan);
    }

    [HttpPost]
    public async Task<ActionResult<TariffPlanDto>> Create([FromBody] CreateTariffPlanRequest request, CancellationToken cancellationToken)
    {
        var plan = await _service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = plan.Id }, plan);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TariffPlanDto>> Update(Guid id, [FromBody] UpdateTariffPlanRequest request, CancellationToken cancellationToken)
    {
        var plan = await _service.UpdateAsync(id, request, cancellationToken);
        if (plan is null) return NotFound();
        return Ok(plan);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _service.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
