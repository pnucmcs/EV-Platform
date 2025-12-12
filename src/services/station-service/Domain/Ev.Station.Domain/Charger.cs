namespace Ev.Station.Domain;

public sealed class Charger
{
    public Guid Id { get; private set; }
    public Guid StationId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public ConnectorType ConnectorType { get; private set; } = ConnectorType.Unknown;
    public ChargerStatus Status { get; private set; } = ChargerStatus.Unknown;
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    private Charger()
    {
        // For deserialization
    }

    private Charger(Guid id, string name, ConnectorType connectorType, ChargerStatus status, Guid stationId)
    {
        Id = id;
        Name = name;
        ConnectorType = connectorType;
        Status = status;
        StationId = stationId;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static Charger Create(string name, ConnectorType connectorType, ChargerStatus status = ChargerStatus.Available)
    {
        return new Charger(Guid.NewGuid(), name, connectorType, status, Guid.Empty);
    }

    public void UpdateStatus(ChargerStatus status)
    {
        Status = status;
        Touch();
    }

    public void UpdateConnectorType(ConnectorType connectorType)
    {
        ConnectorType = connectorType;
        Validate();
        Touch();
    }

    public void UpdateName(string name)
    {
        Name = name;
        Validate();
        Touch();
    }

    public void Validate()
    {
        if (Id == Guid.Empty)
        {
            throw new InvalidOperationException("Charger ID cannot be empty.");
        }

        if (StationId == Guid.Empty)
        {
            throw new InvalidOperationException("Charger must belong to a station.");
        }

        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new InvalidOperationException("Charger name cannot be null or whitespace.");
        }

        if (ConnectorType == ConnectorType.Unknown)
        {
            throw new InvalidOperationException("Connector type must be specified.");
        }
    }

    private void Touch()
    {
        UpdatedAtUtc = DateTime.UtcNow;
    }

    internal void AssignToStation(Guid stationId)
    {
        StationId = stationId;
    }
}
