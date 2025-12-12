using AutoMapper;
using Ev.Station.Application.Dtos;
using Ev.Station.Application.Requests;
using Ev.Station.Domain;

namespace Ev.Station.Application.Services;

public sealed class StationService : IStationService
{
    private readonly IStationRepository _repository;
    private readonly IMapper _mapper;

    public StationService(IStationRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyCollection<StationDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetAllAsync(cancellationToken);
        return entities.Select(_mapper.Map<StationDto>).ToArray();
    }

    public async Task<StationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : _mapper.Map<StationDto>(entity);
    }

    public async Task<StationDto> CreateAsync(CreateStationRequest request, CancellationToken cancellationToken = default)
    {
        var station = new Ev.Station.Domain.Station
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Location = request.Location,
            TotalSpots = request.TotalSpots,
            Status = StationStatus.Online
        };

        await _repository.AddAsync(station, cancellationToken);
        return _mapper.Map<StationDto>(station);
    }
}
