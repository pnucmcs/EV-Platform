using AutoMapper;
using Ev.Notification.Application.Models;
using DomainNotification = Ev.Notification.Domain.Notification;

namespace Ev.Notification.Application.Mapping;

public class NotificationProfile : Profile
{
    public NotificationProfile()
    {
        CreateMap<DomainNotification, NotificationDto>();
    }
}
