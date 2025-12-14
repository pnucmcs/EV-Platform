using Ev.Billing.Application.Models;
using Ev.Billing.Application.Requests;
using Ev.Billing.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ev.Billing.Api.Controllers;

[ApiController]
[Route("api/v1")]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _service;

    public InvoicesController(IInvoiceService service)
    {
        _service = service;
    }

    [HttpGet("invoices/{id:guid}")]
    public async Task<ActionResult<InvoiceDto>> Get(Guid id, CancellationToken cancellationToken)
    {
        var invoice = await _service.GetAsync(id, cancellationToken);
        return invoice is null ? NotFound() : Ok(invoice);
    }

    [HttpGet("users/{userId:guid}/invoices")]
    public async Task<ActionResult<IReadOnlyList<InvoiceDto>>> GetForUser(Guid userId, CancellationToken cancellationToken)
    {
        var invoices = await _service.GetByUserAsync(userId, cancellationToken);
        return Ok(invoices);
    }

    [HttpPost("invoices")]
    public async Task<ActionResult<InvoiceDto>> Create([FromBody] CreateInvoiceRequest request, CancellationToken cancellationToken)
    {
        var invoice = await _service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = invoice.Id }, invoice);
    }

    [HttpPost("invoices/{id:guid}/mark-paid")]
    public async Task<ActionResult<InvoiceDto>> MarkPaid(Guid id, [FromBody] MarkPaidRequest request, CancellationToken cancellationToken)
    {
        var invoice = await _service.MarkPaidAsync(id, request.Provider, cancellationToken);
        return invoice is null ? NotFound() : Ok(invoice);
    }
}
