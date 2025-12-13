using Ev.Notification.Application.Models;
using Ev.Notification.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ev.Notification.Api.Controllers;

[ApiController]
[Route("api/v1/users/{userId:guid}/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _service;

    public NotificationsController(INotificationService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<NotificationDto>>> GetForUser(Guid userId, CancellationToken cancellationToken)
    {
        var items = await _service.GetForUserAsync(userId, cancellationToken);
        return Ok(items);
    }
}
