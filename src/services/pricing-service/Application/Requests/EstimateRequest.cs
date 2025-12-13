using System.ComponentModel.DataAnnotations;

namespace Ev.Pricing.Application.Requests;

public class EstimateRequest
{
    public Guid? StationId { get; set; }
    [Required]
    public DateTime StartTimeUtc { get; set; }
    [Required]
    public DateTime EndTimeUtc { get; set; }
    [Range(0, double.MaxValue)]
    public decimal EstimatedKwh { get; set; }
}

public record EstimateBreakdown(decimal EnergyCost, decimal IdleCost, decimal Total, string Currency);

public record EstimateResponse(decimal Total, string Currency, EstimateBreakdown Breakdown);
