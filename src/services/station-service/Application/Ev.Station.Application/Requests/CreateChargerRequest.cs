using Ev.Station.Domain;

namespace Ev.Station.Application.Requests;

public sealed record CreateChargerRequest(string Name, ConnectorType ConnectorType, ChargerStatus Status = ChargerStatus.Available);
