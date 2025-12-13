using Ev.Notification.Domain;

namespace Ev.Notification.Application.Models;

public record NotificationDto(
    Guid Id,
    Guid? UserId,
    string Type,
    string Title,
    string Body,
    NotificationStatus Status,
    DateTime CreatedAtUtc,
    Guid EventId);
