using System.Text.Json;
using AutoMapper;
using Ev.Notification.Application.Models;
using Ev.Notification.Application.Services;
using Ev.Platform.Contracts;
using Ev.Platform.Contracts.Events;
using Ev.Shared.Messaging.RabbitMq;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using NotificationStatus = Ev.Notification.Domain.NotificationStatus;

namespace Notification.Api.Consumers;

public sealed class NotificationEventHandler : IRabbitMqMessageHandler
{
    private readonly ILogger<NotificationEventHandler> _logger;
    private readonly INotificationService _service;
    private readonly IMapper _mapper;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public NotificationEventHandler(ILogger<NotificationEventHandler> logger, INotificationService service, IMapper mapper)
    {
        _logger = logger;
        _service = service;
        _mapper = mapper;
    }

    public bool CanHandle(string routingKey) =>
        routingKey is EventRoutingKeys.StationCreatedV1
                 or EventRoutingKeys.ReservationCreatedV1
                 or EventRoutingKeys.ChargingSessionStartedV1
                 or EventRoutingKeys.ChargingSessionCompletedV1;

    public async Task HandleAsync(string routingKey, ReadOnlyMemory<byte> body, IBasicProperties properties, CancellationToken cancellationToken)
    {
        try
        {
            if (!TryParse(routingKey, body.Span, out var notification))
            {
                _logger.LogWarning("Failed to parse notification for routingKey={RoutingKey}", routingKey);
                return;
            }

            await _service.AddAsync(notification, cancellationToken);
            _logger.LogInformation("Stored notification type={Type} eventId={EventId} userId={UserId}", notification.Type, notification.EventId, notification.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to handle event routingKey={RoutingKey} correlationId={CorrelationId}", routingKey, properties.CorrelationId);
        }
    }

    private bool TryParse(string routingKey, ReadOnlySpan<byte> body, out NotificationDto notification)
    {
        notification = default!;
        try
        {
            switch (routingKey)
            {
                case EventRoutingKeys.StationCreatedV1:
                    var stationEnvelope = JsonSerializer.Deserialize<EventEnvelope<StationCreatedV1>>(body, _jsonOptions);
                    if (stationEnvelope?.Payload is null) return false;
                    notification = new NotificationDto(
                        Id: Guid.NewGuid(),
                        UserId: null,
                        Type: "station.created",
                        Title: "New station created",
                        Body: $"Station {stationEnvelope.Payload.Name} created at {stationEnvelope.Payload.StationId}",
                        Status: NotificationStatus.Pending,
                        CreatedAtUtc: DateTime.UtcNow,
                        EventId: stationEnvelope.EventId);
                    return true;

                case EventRoutingKeys.ReservationCreatedV1:
                    var resEnvelope = JsonSerializer.Deserialize<EventEnvelope<ReservationCreatedV1>>(body, _jsonOptions);
                    if (resEnvelope?.Payload is null) return false;
                    notification = new NotificationDto(
                        Id: Guid.NewGuid(),
                        UserId: resEnvelope.Payload.UserId,
                        Type: "reservation.created",
                        Title: "Reservation created",
                        Body: $"Reservation {resEnvelope.Payload.ReservationId} at station {resEnvelope.Payload.StationId}",
                        Status: NotificationStatus.Pending,
                        CreatedAtUtc: DateTime.UtcNow,
                        EventId: resEnvelope.EventId);
                    return true;

                case EventRoutingKeys.ChargingSessionStartedV1:
                    var startEnvelope = JsonSerializer.Deserialize<EventEnvelope<ChargingSessionStartedV1>>(body, _jsonOptions);
                    if (startEnvelope?.Payload is null) return false;
                    notification = new NotificationDto(
                        Id: Guid.NewGuid(),
                        UserId: startEnvelope.Payload.ReservationId, // placeholder until userId is available on event
                        Type: "session.started",
                        Title: "Charging session started",
                        Body: $"Session {startEnvelope.Payload.SessionId} started at station {startEnvelope.Payload.StationId}",
                        Status: NotificationStatus.Pending,
                        CreatedAtUtc: DateTime.UtcNow,
                        EventId: startEnvelope.EventId);
                    return true;

                case EventRoutingKeys.ChargingSessionCompletedV1:
                    var endEnvelope = JsonSerializer.Deserialize<EventEnvelope<ChargingSessionCompletedV1>>(body, _jsonOptions);
                    if (endEnvelope?.Payload is null) return false;
                    notification = new NotificationDto(
                        Id: Guid.NewGuid(),
                        UserId: endEnvelope.Payload.ReservationId, // placeholder until userId is on event
                        Type: "session.completed",
                        Title: "Charging session completed",
                        Body: $"Session {endEnvelope.Payload.SessionId} completed. Energy: {endEnvelope.Payload.EnergyKWh ?? 0} kWh",
                        Status: NotificationStatus.Pending,
                        CreatedAtUtc: DateTime.UtcNow,
                        EventId: endEnvelope.EventId);
                    return true;
                default:
                    return false;
            }
        }
        catch
        {
            return false;
        }
    }
}
