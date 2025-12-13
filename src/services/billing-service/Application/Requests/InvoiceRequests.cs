using System.ComponentModel.DataAnnotations;

namespace Ev.Billing.Application.Requests;

public class CreateInvoiceRequest
{
    [Required] public Guid SessionId { get; set; }
    [Required] public Guid UserId { get; set; }
    [Required] public Guid StationId { get; set; }
    [Range(0, double.MaxValue)] public decimal Amount { get; set; }
    [Required] public string Currency { get; set; } = "USD";
}

public class MarkPaidRequest
{
    public string Provider { get; set; } = "manual";
}
