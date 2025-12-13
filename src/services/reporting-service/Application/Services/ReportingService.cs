using AutoMapper;
using Ev.Reporting.Application.Models;
using Ev.Reporting.Domain;
using Ev.Reporting.Domain.Repositories;

namespace Ev.Reporting.Application.Services;

public class ReportingService : IReportingService
{
    private readonly IReportingRepository _repository;
    private readonly IMapper _mapper;

    public ReportingService(IReportingRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<StationDailyReportDto>> GetStationDailyAsync(Guid stationId, DateOnly? from, DateOnly? to, CancellationToken cancellationToken)
    {
        var results = await _repository.GetDailyRangeAsync(stationId, from, to, cancellationToken);
        return _mapper.Map<IReadOnlyList<StationDailyReportDto>>(results);
    }
}
