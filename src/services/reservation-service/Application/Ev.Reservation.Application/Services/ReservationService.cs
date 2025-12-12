using AutoMapper;
using Ev.Reservation.Application.Dtos;
using Ev.Reservation.Application.Requests;
using Ev.Reservation.Domain;
using Ev.Shared.Messaging.Events;
using Ev.Shared.Messaging.RabbitMq;

namespace Ev.Reservation.Application.Services;

public sealed class ReservationService : IReservationService
{
    private readonly IReservationRepository _repository;
    private readonly IMapper _mapper;
    private readonly IRabbitMqPublisher _publisher;

    public ReservationService(IReservationRepository repository, IMapper mapper, IRabbitMqPublisher publisher)
    {
        _repository = repository;
        _mapper = mapper;
        _publisher = publisher;
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
        var reservation = new Domain.Reservation
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            StationId = request.StationId,
            StartsAtUtc = request.StartsAtUtc,
            EndsAtUtc = request.EndsAtUtc,
            Status = ReservationStatus.Created,
            CreatedAtUtc = DateTime.UtcNow
        };
        await _repository.AddAsync(reservation, cancellationToken);
        _publisher.Publish("reservation.created", new ReservationCreated(reservation.Id, reservation.UserId, reservation.StationId, reservation.StartsAtUtc, reservation.EndsAtUtc));
        return _mapper.Map<ReservationDto>(reservation);
    }
}
