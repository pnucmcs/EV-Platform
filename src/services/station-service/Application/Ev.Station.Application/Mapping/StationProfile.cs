using AutoMapper;
using Ev.Station.Application.Dtos;
using Ev.Station.Domain;

namespace Ev.Station.Application.Mapping;

public sealed class StationProfile : Profile
{
    public StationProfile()
    {
        CreateMap<Ev.Station.Domain.Station, StationDto>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));
    }
}
