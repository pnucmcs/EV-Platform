using System.Text.Json;
using Ev.Billing.Api.Services;
using Ev.Billing.Application.Requests;
using Ev.Billing.Application.Services;
using Ev.Platform.Contracts;
using Ev.Platform.Contracts.Events;
using Ev.Shared.Messaging.RabbitMq;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Ev.Billing.Api.Messaging;

public class SessionCompletedEventHandler : IRabbitMqMessageHandler
{
    private readonly IInvoiceService _invoiceService;
    private readonly IPricingClient _pricingClient;
    private readonly ILogger<SessionCompletedEventHandler> _logger;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public SessionCompletedEventHandler(IInvoiceService invoiceService, IPricingClient pricingClient, ILogger<SessionCompletedEventHandler> logger)
    {
        _invoiceService = invoiceService;
        _pricingClient = pricingClient;
        _logger = logger;
    }

    public bool CanHandle(string routingKey) => routingKey == EventRoutingKeys.ChargingSessionCompletedV1;

    public async Task HandleAsync(string routingKey, ReadOnlyMemory<byte> body, IBasicProperties properties, CancellationToken cancellationToken)
    {
        ChargingSessionCompletedV1? payload = null;
        try
        {
            var envelope = JsonSerializer.Deserialize<EventEnvelope<ChargingSessionCompletedV1>>(body.Span, _jsonOptions);
            payload = envelope?.Payload;
            if (payload is null)
            {
                _logger.LogWarning("Received session.completed.v1 with empty payload.");
                return;
            }

            decimal amount = 0;
            var estimate = await _pricingClient.EstimateAsync(payload.StationId, payload.StartedAtUtc, payload.EndedAtUtc, payload.EnergyKWh, cancellationToken);
            if (estimate.HasValue)
            {
                amount = estimate.Value;
            }

            var request = new CreateInvoiceRequest
            {
                SessionId = payload.SessionId,
                UserId = payload.ReservationId, // placeholder until user context is available
                StationId = payload.StationId,
                Amount = amount,
                Currency = "USD"
            };

            var invoice = await _invoiceService.CreateAsync(request, cancellationToken);
            _logger.LogInformation("Created invoice {InvoiceId} for session {SessionId} amount {Amount}", invoice.Id, payload.SessionId, amount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to handle session.completed.v1 for session {SessionId}", payload?.SessionId);
            // swallow to avoid crashing consumer; message is acked by caller regardless
        }
    }
}
