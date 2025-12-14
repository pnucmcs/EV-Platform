using Ev.Reporting.Application.Models;
using Ev.Reporting.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ev.Reporting.Api.Controllers;

[ApiController]
[Route("api/v1/reports/stations/{stationId:guid}/daily")]
public class ReportsController : ControllerBase
{
    private readonly IReportingService _reportingService;

    public ReportsController(IReportingService reportingService)
    {
        _reportingService = reportingService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<StationDailyReportDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<StationDailyReportDto>>> Get(Guid stationId, [FromQuery] DateOnly? from, [FromQuery] DateOnly? to, CancellationToken cancellationToken)
    {
        var result = await _reportingService.GetStationDailyAsync(stationId, from, to, cancellationToken);
        return Ok(result);
    }
}
