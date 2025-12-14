using Microsoft.AspNetCore.Mvc;

namespace Ev.Pricing.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TariffsController : ControllerBase
{
    private static readonly List<TariffDto> Tariffs = new();

    [HttpGet]
    public IActionResult GetAll() => Ok(Tariffs);

    [HttpPost]
    public IActionResult Create([FromBody] CreateTariffRequest request)
    {
        var tariff = new TariffDto(Guid.NewGuid(), request.StationId, request.RatePerKwh, request.Currency);
        Tariffs.Add(tariff);
        return CreatedAtAction(nameof(GetAll), tariff);
    }
}

public record CreateTariffRequest(Guid StationId, decimal RatePerKwh, string Currency);
public record TariffDto(Guid Id, Guid StationId, decimal RatePerKwh, string Currency);
