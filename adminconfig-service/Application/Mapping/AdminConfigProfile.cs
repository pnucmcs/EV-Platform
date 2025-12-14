using AutoMapper;
using Ev.AdminConfig.Application.Models;
using Ev.AdminConfig.Application.Requests;
using Ev.AdminConfig.Domain;

namespace Ev.AdminConfig.Application.Mapping;

public class AdminConfigProfile : Profile
{
    public AdminConfigProfile()
    {
        CreateMap<CreateMaintenanceWindowRequest, MaintenanceWindow>();
        CreateMap<UpdateMaintenanceWindowRequest, MaintenanceWindow>();
        CreateMap<MaintenanceWindow, MaintenanceWindowDto>();

        CreateMap<StationOpsConfigRequest, StationOpsConfig>();
        CreateMap<StationOpsConfig, StationOpsConfigDto>();
    }
}
