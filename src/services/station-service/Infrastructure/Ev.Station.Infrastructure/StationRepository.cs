using Ev.Station.Domain;
using MongoDB.Driver;
using StackExchange.Redis;

namespace Ev.Station.Infrastructure;

public sealed class StationRepository : IStationRepository
{
    private readonly IMongoCollection<Domain.Station> _collection;
    private readonly IDatabase _cache;

    public StationRepository(StationMongoSettings settings, IConnectionMultiplexer redis)
    {
        var client = new MongoClient(settings.ConnectionString);
        var db = client.GetDatabase(settings.Database);
        _collection = db.GetCollection<Domain.Station>(settings.Collection);
        _cache = redis.GetDatabase();
    }

    public async Task AddAsync(Domain.Station station, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(station, cancellationToken: cancellationToken);
        await CacheAsync(station, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Domain.Station>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var stations = await _collection.Find(_ => true).ToListAsync(cancellationToken);
        return stations;
    }

    public async Task<Domain.Station?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cached = await GetCachedAsync(id, cancellationToken);
        if (cached is not null) return cached;

        var station = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
        if (station is not null)
        {
            await CacheAsync(station, cancellationToken);
        }
        return station;
    }

    private async Task CacheAsync(Domain.Station station, CancellationToken cancellationToken)
    {
        var key = $"station:{station.Id}";
        await _cache.StringSetAsync(key, System.Text.Json.JsonSerializer.Serialize(station), TimeSpan.FromMinutes(5));
    }

    private async Task<Domain.Station?> GetCachedAsync(Guid id, CancellationToken cancellationToken)
    {
        var key = $"station:{id}";
        var value = await _cache.StringGetAsync(key);
        if (value.IsNullOrEmpty) return null;
        return System.Text.Json.JsonSerializer.Deserialize<Domain.Station>(value!);
    }
}
