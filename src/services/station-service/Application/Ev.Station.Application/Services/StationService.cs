using AutoMapper;
using Ev.Station.Application.Dtos;
using Ev.Station.Application.Requests;
using Ev.Station.Domain;

namespace Ev.Station.Application.Services;

public sealed class StationService : IStationService
{
    private readonly IStationRepository _repository;
    private readonly IMapper _mapper;

    public StationService(IStationRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyCollection<StationDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetAllAsync(cancellationToken);
        return entities.Select(_mapper.Map<StationDto>).ToArray();
    }

    public async Task<StationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : _mapper.Map<StationDto>(entity);
    }

    public async Task<StationDto> CreateAsync(CreateStationRequest request, CancellationToken cancellationToken = default)
    {
        var station = Ev.Station.Domain.Station.Create(
            request.Name,
            request.Latitude,
            request.Longitude,
            StationStatus.Online);

        var correlationId = System.Diagnostics.Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString();
        var evt = new Ev.Platform.Contracts.Events.StationCreatedV1
        {
            StationId = station.Id,
            Name = station.Name,
            Latitude = station.Latitude,
            Longitude = station.Longitude,
            Status = station.Status.ToString()
        };
        var envelope = new Ev.Platform.Contracts.EventEnvelope<Ev.Platform.Contracts.Events.StationCreatedV1>(evt, correlationId: correlationId, producer: "station-service@1.0.0");
        var outbox = new Ev.Station.Domain.OutboxMessage
        {
            Id = envelope.EventId,
            OccurredAtUtc = envelope.OccurredAtUtc,
            Type = envelope.EventType,
            RoutingKey = Ev.Platform.Contracts.EventRoutingKeys.StationCreatedV1,
            PayloadJson = System.Text.Json.JsonSerializer.Serialize(envelope),
            CorrelationId = envelope.CorrelationId
        };

        await _repository.AddAsync(station, new[] { outbox }, cancellationToken);
        return _mapper.Map<StationDto>(station);
    }

    public async Task<StationDto?> UpdateAsync(Guid id, UpdateStationRequest request, CancellationToken cancellationToken = default)
    {
        var station = await _repository.GetByIdAsync(id, cancellationToken);
        if (station is null)
        {
            return null;
        }

        var previousStatus = station.Status;
        station.UpdateName(request.Name);
        station.UpdateLocation(request.Latitude, request.Longitude);
        station.UpdateStatus(request.Status);

        var outboxMessages = new List<object>();
        if (previousStatus != request.Status)
        {
            var correlationId = System.Diagnostics.Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString();
            var evt = new Ev.Platform.Contracts.Events.StationStatusChangedV1
            {
                StationId = station.Id,
                PreviousStatus = previousStatus.ToString(),
                NewStatus = request.Status.ToString(),
                OccurredAtUtc = DateTime.UtcNow
            };
            var envelope = new Ev.Platform.Contracts.EventEnvelope<Ev.Platform.Contracts.Events.StationStatusChangedV1>(evt, correlationId: correlationId, producer: "station-service@1.0.0");
            outboxMessages.Add(new Ev.Station.Domain.OutboxMessage
            {
                Id = envelope.EventId,
                OccurredAtUtc = envelope.OccurredAtUtc,
                Type = envelope.EventType,
                RoutingKey = Ev.Platform.Contracts.EventRoutingKeys.StationStatusChangedV1,
                PayloadJson = System.Text.Json.JsonSerializer.Serialize(envelope),
                CorrelationId = envelope.CorrelationId
            });
        }

        await _repository.UpdateAsync(station, outboxMessages, cancellationToken);
        return _mapper.Map<StationDto>(station);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<ChargerDto>?> GetChargersAsync(Guid stationId, CancellationToken cancellationToken = default)
    {
        var station = await _repository.GetByIdAsync(stationId, cancellationToken);
        return station is null ? null : station.Chargers.Select(_mapper.Map<ChargerDto>).ToArray();
    }

    public async Task<ChargerDto?> GetChargerByIdAsync(Guid stationId, Guid chargerId, CancellationToken cancellationToken = default)
    {
        var station = await _repository.GetByIdAsync(stationId, cancellationToken);
        var charger = station?.GetCharger(chargerId);
        return charger is null ? null : _mapper.Map<ChargerDto>(charger);
    }

    public async Task<ChargerDto?> CreateChargerAsync(Guid stationId, CreateChargerRequest request, CancellationToken cancellationToken = default)
    {
        var station = await _repository.GetByIdAsync(stationId, cancellationToken);
        if (station is null)
        {
            return null;
        }

        var charger = Ev.Station.Domain.Charger.Create(request.Name, request.ConnectorType, request.Status);
        station.AddCharger(charger);
        await _repository.UpdateAsync(station, null, cancellationToken);

        return _mapper.Map<ChargerDto>(charger);
    }

    public async Task<ChargerDto?> UpdateChargerAsync(Guid stationId, Guid chargerId, UpdateChargerRequest request, CancellationToken cancellationToken = default)
    {
        var station = await _repository.GetByIdAsync(stationId, cancellationToken);
        if (station is null)
        {
            return null;
        }

        var charger = station.GetCharger(chargerId);
        if (charger is null)
        {
            return null;
        }

        charger.UpdateName(request.Name);
        charger.UpdateConnectorType(request.ConnectorType);
        charger.UpdateStatus(request.Status);

        await _repository.UpdateAsync(station, null, cancellationToken);
        return _mapper.Map<ChargerDto>(charger);
    }

    public async Task<bool> DeleteChargerAsync(Guid stationId, Guid chargerId, CancellationToken cancellationToken = default)
    {
        var station = await _repository.GetByIdAsync(stationId, cancellationToken);
        if (station is null)
        {
            return false;
        }

        var removed = station.RemoveCharger(chargerId);
        if (!removed)
        {
            return false;
        }

        await _repository.UpdateAsync(station, null, cancellationToken);
        return true;
    }
}
