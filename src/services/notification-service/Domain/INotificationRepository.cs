namespace Ev.Notification.Domain;

public interface INotificationRepository
{
    Task<bool> ExistsForEventAsync(Guid eventId, CancellationToken cancellationToken);
    Task AddAsync(Notification notification, CancellationToken cancellationToken);
    Task<IReadOnlyList<Notification>> GetForUserAsync(Guid userId, CancellationToken cancellationToken);
}
