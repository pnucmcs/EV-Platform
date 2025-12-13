using AutoMapper;
using Ev.Telemetry.Application.Models;
using Ev.Telemetry.Application.Requests;
using Ev.Telemetry.Domain;

namespace Ev.Telemetry.Application.Mapping;

public class TelemetryProfile : Profile
{
    public TelemetryProfile()
    {
        CreateMap<TelemetryReadingRequest, TelemetryReading>();
        CreateMap<TelemetryReading, TelemetryReadingDto>();
    }
}
