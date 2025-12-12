using Microsoft.AspNetCore.Mvc;

namespace Ev.Billing.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private static readonly List<InvoiceDto> Invoices = new();

    [HttpGet]
    public IActionResult GetAll() => Ok(Invoices);

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        var inv = Invoices.FirstOrDefault(x => x.Id == id);
        return inv is null ? NotFound() : Ok(inv);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateInvoiceRequest request)
    {
        var inv = new InvoiceDto(Guid.NewGuid(), request.ReservationId, request.Amount, request.Currency, "Created", DateTime.UtcNow);
        Invoices.Add(inv);
        return CreatedAtAction(nameof(GetById), new { id = inv.Id }, inv);
    }
}

public record CreateInvoiceRequest(Guid ReservationId, decimal Amount, string Currency);
public record InvoiceDto(Guid Id, Guid ReservationId, decimal Amount, string Currency, string Status, DateTime CreatedAtUtc);
