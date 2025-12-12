using AutoMapper;
using Ev.Reservation.Application.Dtos;
using Ev.Reservation.Domain;

namespace Ev.Reservation.Application.Mapping;

public sealed class ReservationProfile : Profile
{
    public ReservationProfile()
    {
        CreateMap<Ev.Reservation.Domain.Reservation, ReservationDto>();
        CreateMap<Ev.Reservation.Domain.ChargingSession, ChargingSessionDto>();
    }
}
