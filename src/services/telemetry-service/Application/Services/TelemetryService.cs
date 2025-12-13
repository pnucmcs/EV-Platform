using AutoMapper;
using Ev.Telemetry.Application.Models;
using Ev.Telemetry.Application.Requests;
using Ev.Telemetry.Domain;

namespace Ev.Telemetry.Application.Services;

public class TelemetryService : ITelemetryService
{
    private readonly ITelemetryRepository _repository;
    private readonly IMapper _mapper;

    public TelemetryService(ITelemetryRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task IngestAsync(IEnumerable<TelemetryReadingRequest> readings, CancellationToken cancellationToken)
    {
        var models = readings.Select(r => _mapper.Map<TelemetryReading>(r)).ToList();
        foreach (var model in models)
        {
            model.Validate();
        }
        await _repository.AddReadingsAsync(models, cancellationToken);
    }

    public async Task<TelemetryReadingDto?> GetLatestForStationAsync(Guid stationId, CancellationToken cancellationToken)
    {
        var reading = await _repository.GetLatestForStationAsync(stationId, cancellationToken);
        return reading is null ? null : _mapper.Map<TelemetryReadingDto>(reading);
    }
}
