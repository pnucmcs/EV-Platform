using Ev.Notification.Domain;
using Ev.Notification.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ev.Notification.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly NotificationDbContext _db;

    public NotificationRepository(NotificationDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Notification.Domain.Notification notification, CancellationToken cancellationToken)
    {
        await _db.Notifications.AddAsync(notification, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> ExistsForEventAsync(Guid eventId, CancellationToken cancellationToken)
    {
        return _db.Notifications.AnyAsync(n => n.EventId == eventId, cancellationToken);
    }

    public async Task<IReadOnlyList<Notification.Domain.Notification>> GetForUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _db.Notifications.Where(n => n.UserId == userId).OrderByDescending(n => n.CreatedAtUtc).ToListAsync(cancellationToken);
    }
}
