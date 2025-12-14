namespace Ev.Station.Infrastructure.Outbox;

public sealed class OutboxOptions
{
    public int BatchSize { get; set; } = 20;
    public int PollIntervalSeconds { get; set; } = 5;
    public int MaxPublishAttempts { get; set; } = 5;
}
