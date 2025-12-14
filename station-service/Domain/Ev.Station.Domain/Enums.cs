namespace Ev.Station.Domain;

public enum StationStatus
{
    Unknown = 0,
    Online = 1,
    Offline = 2,
    Maintenance = 3
}

public enum ChargerStatus
{
    Unknown = 0,
    Available = 1,
    Charging = 2,
    Faulted = 3,
    OutOfService = 4,
    Maintenance = 5
}

public enum ConnectorType
{
    Unknown = 0,
    Type1 = 1,
    Type2 = 2,
    CCS1 = 3,
    CCS2 = 4,
    CHAdeMO = 5,
    TeslaNorthAmerican = 6,
    GB_T = 7
}
