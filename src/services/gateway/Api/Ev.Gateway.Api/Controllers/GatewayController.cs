using Microsoft.AspNetCore.Mvc;

namespace Ev.Gateway.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GatewayController : ControllerBase
{
    [HttpGet("services")]
    public IActionResult Services()
    {
        var services = new[]
        {
            "auth-service",
            "user-service",
            "station-service",
            "pricing-service",
            "reservation-service",
            "billing-service",
            "notification-service",
            "telemetry-service",
            "reporting-service"
        };
        return Ok(services);
    }
}
