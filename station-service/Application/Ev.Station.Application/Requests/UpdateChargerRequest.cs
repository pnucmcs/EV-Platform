using Ev.Station.Domain;

namespace Ev.Station.Application.Requests;

public sealed record UpdateChargerRequest(string Name, ConnectorType ConnectorType, ChargerStatus Status);
