using AutoMapper;
using Ev.Notification.Application.Models;
using Ev.Notification.Domain;

namespace Ev.Notification.Application.Mapping;

public class NotificationProfile : Profile
{
    public NotificationProfile()
    {
        CreateMap<Notification, NotificationDto>();
    }
}
