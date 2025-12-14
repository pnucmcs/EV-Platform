using Ev.Notification.Application.Models;

namespace Ev.Notification.Application.Services;

public interface INotificationService
{
    Task AddAsync(NotificationDto notification, CancellationToken cancellationToken);
    Task<IReadOnlyList<NotificationDto>> GetForUserAsync(Guid userId, CancellationToken cancellationToken);
}
