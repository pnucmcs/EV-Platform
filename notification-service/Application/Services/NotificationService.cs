using AutoMapper;
using Ev.Notification.Application.Models;
using Ev.Notification.Domain;

namespace Ev.Notification.Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _repository;
    private readonly IMapper _mapper;

    public NotificationService(INotificationRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task AddAsync(NotificationDto notificationDto, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<Notification.Domain.Notification>(notificationDto);
        entity.Validate();
        if (await _repository.ExistsForEventAsync(entity.EventId, cancellationToken))
        {
            return;
        }
        await _repository.AddAsync(entity, cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationDto>> GetForUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var entities = await _repository.GetForUserAsync(userId, cancellationToken);
        return _mapper.Map<IReadOnlyList<NotificationDto>>(entities);
    }
}
