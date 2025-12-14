using Ev.Pricing.Application.Requests;
using Ev.Pricing.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ev.Pricing.Api.Controllers;

[ApiController]
[Route("api/v1/pricing")]
public class PricingController : ControllerBase
{
    private readonly ITariffPlanService _service;

    public PricingController(ITariffPlanService service)
    {
        _service = service;
    }

    [HttpPost("estimate")]
    public async Task<ActionResult<EstimateResponse>> Estimate([FromBody] EstimateRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.EstimateAsync(request, cancellationToken);
        if (result is null) return NotFound(new ProblemDetails { Title = "No active tariff plan found" });
        return Ok(result);
    }
}
