using System.Text.Json;
using Ev.Platform.Contracts;
using Ev.Platform.Contracts.Events;
using Ev.Reporting.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Ev.Shared.Messaging.RabbitMq;

namespace Ev.Reporting.Api.Messaging;

public class ReportingEventsHandler : IRabbitMqMessageHandler
{
    private readonly IReportingRepository _repository;
    private readonly ILogger<ReportingEventsHandler> _logger;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    private static readonly HashSet<string> SupportedKeys = new()
    {
        EventRoutingKeys.StationCreatedV1,
        EventRoutingKeys.StationStatusChangedV1,
        EventRoutingKeys.ReservationCreatedV1,
        EventRoutingKeys.ReservationCancelledV1,
        EventRoutingKeys.ChargingSessionStartedV1,
        EventRoutingKeys.ChargingSessionCompletedV1
    };

    public ReportingEventsHandler(IReportingRepository repository, ILogger<ReportingEventsHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public bool CanHandle(string routingKey) => SupportedKeys.Contains(routingKey);

    public async Task HandleAsync(string routingKey, ReadOnlyMemory<byte> body, IBasicProperties properties, CancellationToken cancellationToken)
    {
        try
        {
            switch (routingKey)
            {
                case var key when key == EventRoutingKeys.ChargingSessionStartedV1:
                    await HandleSessionStarted(body, cancellationToken);
                    break;
                case var key when key == EventRoutingKeys.ChargingSessionCompletedV1:
                    await HandleSessionCompleted(body, cancellationToken);
                    break;
                default:
                    await MarkGenericProcessedAsync(body, cancellationToken);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to handle event {RoutingKey}", routingKey);
        }
    }

    private async Task HandleSessionStarted(ReadOnlyMemory<byte> body, CancellationToken cancellationToken)
    {
        var envelope = JsonSerializer.Deserialize<EventEnvelope<ChargingSessionStartedV1>>(body.Span, _jsonOptions);
        if (envelope?.Payload is null)
        {
            _logger.LogWarning("Received session.started with empty payload");
            return;
        }

        if (await _repository.HasProcessedEventAsync(envelope.EventId, cancellationToken))
        {
            return;
        }

        var date = DateOnly.FromDateTime(envelope.Payload.StartedAtUtc.Date);
        var daily = await _repository.GetOrCreateDailyAsync(envelope.Payload.StationId, date, cancellationToken);
        daily.SessionsCount += 1;
        daily.UpdatedAtUtc = DateTime.UtcNow;
        daily.Validate();

        await _repository.MarkEventProcessedAsync(envelope.EventId, cancellationToken);
        await SaveIgnoringDuplicateEventAsync(envelope.EventId, cancellationToken);
    }

    private async Task HandleSessionCompleted(ReadOnlyMemory<byte> body, CancellationToken cancellationToken)
    {
        var envelope = JsonSerializer.Deserialize<EventEnvelope<ChargingSessionCompletedV1>>(body.Span, _jsonOptions);
        if (envelope?.Payload is null)
        {
            _logger.LogWarning("Received session.completed with empty payload");
            return;
        }

        if (await _repository.HasProcessedEventAsync(envelope.EventId, cancellationToken))
        {
            return;
        }

        var date = DateOnly.FromDateTime(envelope.Payload.StartedAtUtc.Date);
        var daily = await _repository.GetOrCreateDailyAsync(envelope.Payload.StationId, date, cancellationToken);
        if (envelope.Payload.EnergyKWh.HasValue)
        {
            daily.TotalKwh += envelope.Payload.EnergyKWh.Value;
        }
        daily.UpdatedAtUtc = DateTime.UtcNow;
        daily.Validate();

        await _repository.MarkEventProcessedAsync(envelope.EventId, cancellationToken);
        await SaveIgnoringDuplicateEventAsync(envelope.EventId, cancellationToken);
    }

    private async Task SaveIgnoringDuplicateEventAsync(Guid eventId, CancellationToken cancellationToken)
    {
        try
        {
            await _repository.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogWarning(dbEx, "Duplicate processing attempt for event {EventId}", eventId);
        }
    }

    private async Task MarkGenericProcessedAsync(ReadOnlyMemory<byte> body, CancellationToken cancellationToken)
    {
        try
        {
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("eventId", out var idElement) &&
                Guid.TryParse(idElement.GetString(), out var eventId))
            {
                if (await _repository.HasProcessedEventAsync(eventId, cancellationToken))
                {
                    return;
                }

                await _repository.MarkEventProcessedAsync(eventId, cancellationToken);
                await SaveIgnoringDuplicateEventAsync(eventId, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to mark generic event processed");
        }
    }
}
