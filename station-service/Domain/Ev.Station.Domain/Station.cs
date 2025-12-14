namespace Ev.Station.Domain;

public sealed class Station
{
    private readonly List<Charger> _chargers = new();

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public StationStatus Status { get; private set; } = StationStatus.Unknown;
    public IReadOnlyCollection<Charger> Chargers => _chargers.AsReadOnly();
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    private Station()
    {
        // For deserialization
    }

    private Station(Guid id, string name, double latitude, double longitude, StationStatus status, IEnumerable<Charger>? chargers = null)
    {
        Id = id;
        Name = name;
        Latitude = latitude;
        Longitude = longitude;
        Status = status;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = null;

        if (chargers is not null)
        {
            foreach (var charger in chargers)
            {
                AddCharger(charger);
            }
        }

        Validate();
    }

    public static Station Create(string name, double latitude, double longitude, StationStatus status = StationStatus.Online, IEnumerable<Charger>? chargers = null)
    {
        return new Station(Guid.NewGuid(), name, latitude, longitude, status, chargers);
    }

    public void UpdateName(string name)
    {
        Name = name;
        Validate();
        Touch();
    }

    public void UpdateLocation(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
        ValidateCoordinates();
        Touch();
    }

    public void UpdateStatus(StationStatus status)
    {
        Status = status;
        Touch();
    }

    public void AddCharger(Charger charger)
    {
        ArgumentNullException.ThrowIfNull(charger);
        if (charger.StationId != Guid.Empty && charger.StationId != Id)
        {
            throw new InvalidOperationException("Charger already belongs to a different station.");
        }
        charger.AssignToStation(Id);
        charger.Validate();
        _chargers.Add(charger);
        Touch();
    }

    public Charger? GetCharger(Guid chargerId) => _chargers.FirstOrDefault(c => c.Id == chargerId);

    public bool RemoveCharger(Guid chargerId)
    {
        var removed = _chargers.RemoveAll(c => c.Id == chargerId) > 0;
        if (removed)
        {
            Touch();
        }
        return removed;
    }

    public void Validate()
    {
        if (Id == Guid.Empty)
        {
            throw new InvalidOperationException("Station ID cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new InvalidOperationException("Station name cannot be null or whitespace.");
        }

        ValidateCoordinates();
    }

    private void ValidateCoordinates()
    {
        if (Latitude is < -90 or > 90)
        {
            throw new InvalidOperationException("Latitude must be between -90 and 90.");
        }

        if (Longitude is < -180 or > 180)
        {
            throw new InvalidOperationException("Longitude must be between -180 and 180.");
        }
    }

    private void Touch()
    {
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
