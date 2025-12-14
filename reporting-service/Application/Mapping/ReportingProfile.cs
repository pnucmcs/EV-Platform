using AutoMapper;
using Ev.Reporting.Application.Models;
using Ev.Reporting.Domain;

namespace Ev.Reporting.Application.Mapping;

public class ReportingProfile : Profile
{
    public ReportingProfile()
    {
        CreateMap<StationUtilizationDaily, StationDailyReportDto>();
    }
}
