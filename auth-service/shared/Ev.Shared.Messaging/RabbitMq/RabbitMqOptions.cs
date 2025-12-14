namespace Ev.Shared.Messaging.RabbitMq;

public sealed class RabbitMqOptions
{
    public string HostName { get; set; } = "localhost";
    public string Host
    {
        get => HostName;
        set => HostName = value;
    }
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string Exchange { get; set; } = "ev.platform";
    public ushort PrefetchCount { get; set; } = 10;
}
