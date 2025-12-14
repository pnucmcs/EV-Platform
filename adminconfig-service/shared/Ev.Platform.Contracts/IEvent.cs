namespace Ev.Platform.Contracts;

public interface IEvent
{
    string EventType { get; }
    int EventVersion { get; }
}
