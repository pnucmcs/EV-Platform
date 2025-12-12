using Microsoft.AspNetCore.Mvc;

namespace Ev.Notification.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private static readonly List<NotificationDto> Notifications = new();

    [HttpGet]
    public IActionResult GetAll() => Ok(Notifications);

    [HttpPost]
    public IActionResult Send([FromBody] SendNotificationRequest request)
    {
        var note = new NotificationDto(Guid.NewGuid(), request.Target, request.Subject, request.Message, DateTime.UtcNow);
        Notifications.Add(note);
        return Ok(note);
    }
}

public record SendNotificationRequest(string Target, string Subject, string Message);
public record NotificationDto(Guid Id, string Target, string Subject, string Message, DateTime SentAtUtc);
