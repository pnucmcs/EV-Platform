using Microsoft.AspNetCore.Mvc;

namespace Ev.Reporting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    [HttpGet("summary")]
    public IActionResult Summary()
    {
        var summary = new
        {
            Reservations = 0,
            ActiveStations = 0,
            PendingInvoices = 0,
            GeneratedAtUtc = DateTime.UtcNow
        };
        return Ok(summary);
    }
}
