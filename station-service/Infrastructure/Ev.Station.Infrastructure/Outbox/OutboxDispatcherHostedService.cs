using System.Text;
using Ev.Shared.Messaging.RabbitMq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ev.Station.Infrastructure.Outbox;

public sealed class OutboxDispatcherHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IRabbitMqPublisher _publisher;
    private readonly OutboxOptions _options;
    private readonly ILogger<OutboxDispatcherHostedService> _logger;

    public OutboxDispatcherHostedService(
        IServiceScopeFactory scopeFactory,
        IRabbitMqPublisher publisher,
        IOptions<OutboxOptions> options,
        ILogger<OutboxDispatcherHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _publisher = publisher;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await DispatchBatchAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Outbox dispatcher encountered an error.");
            }

            await Task.Delay(TimeSpan.FromSeconds(_options.PollIntervalSeconds), stoppingToken);
        }
    }

    private async Task DispatchBatchAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Persistence.StationDbContext>();

        var messages = await db.OutboxMessages
            .Where(x => x.ProcessedAtUtc == null && x.PublishAttempts < _options.MaxPublishAttempts)
            .OrderBy(x => x.OccurredAtUtc)
            .Take(_options.BatchSize)
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                var body = Encoding.UTF8.GetBytes(message.PayloadJson);
                await _publisher.PublishRawAsync(message.RoutingKey, body, message.Id, message.CorrelationId, message.Type, cancellationToken);

                message.ProcessedAtUtc = DateTime.UtcNow;
                message.LastError = null;
                message.PublishAttempts += 1;
            }
            catch (Exception ex)
            {
                message.PublishAttempts += 1;
                message.LastError = ex.Message;
                _logger.LogWarning(ex, "Failed to publish outbox message {MessageId} routingKey={RoutingKey}", message.Id, message.RoutingKey);
            }
        }

        if (messages.Count > 0)
        {
            await db.SaveChangesAsync(cancellationToken);
        }
    }
}
