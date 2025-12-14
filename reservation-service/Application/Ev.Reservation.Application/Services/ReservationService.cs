using AutoMapper;
using Ev.Reservation.Application.Dtos;
using Ev.Reservation.Application.Requests;
using Ev.Reservation.Domain;
using Ev.Platform.Contracts;
using Ev.Platform.Contracts.Events;

namespace Ev.Reservation.Application.Services;

public sealed class ReservationService : IReservationService
{
    private readonly IReservationRepository _repository;
    private readonly IMapper _mapper;
    private readonly IStationDirectoryClient _stationDirectory;

    public ReservationService(IReservationRepository repository, IMapper mapper, IStationDirectoryClient stationDirectory)
    {
        _repository = repository;
        _mapper = mapper;
        _stationDirectory = stationDirectory;
    }

    public async Task<IReadOnlyCollection<ReservationDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return items.Select(_mapper.Map<ReservationDto>).ToArray();
    }

    public async Task<ReservationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : _mapper.Map<ReservationDto>(entity);
    }

    public async Task<ReservationDto> CreateAsync(CreateReservationRequest request, CancellationToken cancellationToken = default)
    {
        var stationExists = await _stationDirectory.ExistsAsync(request.StationId, cancellationToken);
        if (!stationExists)
        {
            throw new InvalidOperationException("Station not found.");
        }

        var hasOverlap = await _repository.HasOverlapAsync(request.StationId, request.StartsAtUtc, request.EndsAtUtc, cancellationToken);
        if (hasOverlap)
        {
            throw new InvalidOperationException("Overlapping reservation exists for this station and time window.");
        }

        var reservation = Domain.Reservation.Create(request.UserId, request.StationId, request.StartsAtUtc, request.EndsAtUtc);

        var evt = new ReservationCreatedV1
        {
            ReservationId = reservation.Id,
            StationId = reservation.StationId,
            UserId = reservation.UserId,
            StartsAtUtc = reservation.StartsAtUtc,
            EndsAtUtc = reservation.EndsAtUtc,
            Status = reservation.Status.ToString()
        };
        var correlationId = System.Diagnostics.Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString();
        var envelope = new EventEnvelope<ReservationCreatedV1>(evt, correlationId: correlationId, producer: "reservation-service@1.0.0");
        var outbox = new Domain.OutboxMessage
        {
            Id = envelope.EventId,
            OccurredAtUtc = envelope.OccurredAtUtc,
            Type = envelope.EventType,
            RoutingKey = EventRoutingKeys.ReservationCreatedV1,
            PayloadJson = System.Text.Json.JsonSerializer.Serialize(envelope),
            CorrelationId = envelope.CorrelationId
        };

        await _repository.AddAsync(reservation, new[] { outbox }, cancellationToken);
        return _mapper.Map<ReservationDto>(reservation);
    }

    public async Task<IReadOnlyCollection<ChargingSessionDto>> GetSessionsAsync(Guid reservationId, CancellationToken cancellationToken = default)
    {
        var sessions = await _repository.GetSessionsAsync(reservationId, cancellationToken);
        return sessions.Select(_mapper.Map<ChargingSessionDto>).ToArray();
    }

    public async Task<ChargingSessionDto?> GetSessionByIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var session = await _repository.GetSessionByIdAsync(sessionId, cancellationToken);
        return session is null ? null : _mapper.Map<ChargingSessionDto>(session);
    }

    public async Task<ChargingSessionDto> CreateSessionAsync(CreateChargingSessionRequest request, CancellationToken cancellationToken = default)
    {
        var reservation = await _repository.GetByIdAsync(request.ReservationId, cancellationToken);
        if (reservation is null)
        {
            throw new InvalidOperationException("Reservation not found.");
        }

        if (reservation.StationId != request.StationId)
        {
            throw new InvalidOperationException("Session station must match reservation station.");
        }

        var hasActive = await _repository.HasActiveSessionAsync(request.ReservationId, cancellationToken);
        if (hasActive)
        {
            throw new InvalidOperationException("An active charging session already exists for this reservation.");
        }

        var session = Domain.ChargingSession.Start(request.ReservationId, request.StationId, request.ChargerId, request.StartedAtUtc);

        var evt = new ChargingSessionStartedV1
        {
            SessionId = session.Id,
            ReservationId = session.ReservationId,
            StationId = session.StationId,
            ChargerId = session.ChargerId,
            StartedAtUtc = session.StartedAtUtc
        };
        var correlationId = System.Diagnostics.Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString();
        var envelope = new EventEnvelope<ChargingSessionStartedV1>(evt, correlationId: correlationId, producer: "reservation-service@1.0.0");
        var outbox = new Domain.OutboxMessage
        {
            Id = envelope.EventId,
            OccurredAtUtc = envelope.OccurredAtUtc,
            Type = envelope.EventType,
            RoutingKey = EventRoutingKeys.ChargingSessionStartedV1,
            PayloadJson = System.Text.Json.JsonSerializer.Serialize(envelope),
            CorrelationId = envelope.CorrelationId
        };

        await _repository.AddSessionAsync(session, new[] { outbox }, cancellationToken);
        return _mapper.Map<ChargingSessionDto>(session);
    }

    public async Task<ChargingSessionDto?> UpdateSessionAsync(Guid sessionId, UpdateChargingSessionRequest request, CancellationToken cancellationToken = default)
    {
        var session = await _repository.GetSessionByIdAsync(sessionId, cancellationToken);
        if (session is null)
        {
            return null;
        }

        var outboxMessages = new List<object>();

        switch (request.Status)
        {
            case ChargingSessionStatus.Completed:
                session.Complete(request.EndedAtUtc ?? DateTime.UtcNow);
                outboxMessages.Add(BuildSessionCompletedOutbox(session));
                break;
            case ChargingSessionStatus.Failed:
                session.Fail();
                break;
            case ChargingSessionStatus.Cancelled:
                session.Cancel();
                break;
            default:
                session.Complete(request.EndedAtUtc ?? DateTime.UtcNow);
                outboxMessages.Add(BuildSessionCompletedOutbox(session));
                break;
        }

        await _repository.UpdateSessionAsync(session, outboxMessages, cancellationToken);
        return _mapper.Map<ChargingSessionDto>(session);
    }

    private Domain.OutboxMessage BuildSessionCompletedOutbox(Domain.ChargingSession session)
    {
        var evt = new ChargingSessionCompletedV1
        {
            SessionId = session.Id,
            ReservationId = session.ReservationId,
            StationId = session.StationId,
            ChargerId = session.ChargerId,
            StartedAtUtc = session.StartedAtUtc,
            EndedAtUtc = session.EndedAtUtc ?? DateTime.UtcNow,
            EnergyKWh = null
        };
        var correlationId = System.Diagnostics.Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString();
        var envelope = new EventEnvelope<ChargingSessionCompletedV1>(evt, correlationId: correlationId, producer: "reservation-service@1.0.0");
        return new Domain.OutboxMessage
        {
            Id = envelope.EventId,
            OccurredAtUtc = envelope.OccurredAtUtc,
            Type = envelope.EventType,
            RoutingKey = EventRoutingKeys.ChargingSessionCompletedV1,
            PayloadJson = System.Text.Json.JsonSerializer.Serialize(envelope),
            CorrelationId = envelope.CorrelationId
        };
    }
}
