using System.Diagnostics;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using OpenTelemetry;
using OpenTelemetry.Context;
using OpenTelemetry.Context.Propagation;

namespace Ev.Shared.Messaging.RabbitMq;

public static class RabbitMqTelemetry
{
    public const string ActivitySourceName = "Ev.Messaging";
    public static readonly ActivitySource ActivitySource = new(ActivitySourceName);
    public static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;

    private static IEnumerable<string> HeaderValues(IDictionary<string, object>? headers, string key)
    {
        if (headers is null || !headers.TryGetValue(key, out var value))
        {
            return Enumerable.Empty<string>();
        }

        return value switch
        {
            byte[] bytes => new[] { Encoding.UTF8.GetString(bytes) },
            string s => new[] { s },
            _ => Enumerable.Empty<string>()
        };
    }

    public static void Inject(Activity activity, IBasicProperties properties)
    {
        var headers = properties.Headers ??= new Dictionary<string, object>();
        Propagator.Inject(new PropagationContext(activity.Context, Baggage.Current), headers,
            static (dict, key, value) => dict[key] = Encoding.UTF8.GetBytes(value));
    }

    public static Activity? StartPublishActivity(string routingKey, string? eventType)
    {
        var activity = ActivitySource.StartActivity($"rabbitmq publish {routingKey}", ActivityKind.Producer);
        if (activity == null)
        {
            return null;
        }

        activity.SetTag("messaging.system", "rabbitmq");
        activity.SetTag("messaging.destination", "ev.platform");
        activity.SetTag("messaging.destination_kind", "topic");
        activity.SetTag("messaging.rabbitmq.routing_key", routingKey);
        if (!string.IsNullOrWhiteSpace(eventType))
        {
            activity.SetTag("messaging.operation.name", eventType);
        }

        return activity;
    }

    public static Activity? StartConsumeActivity(BasicDeliverEventArgs args)
    {
        var parentContext = Propagator.Extract(default, args.BasicProperties.Headers,
            static (dict, key) => HeaderValues(dict, key));
        Baggage.Current = parentContext.Baggage;

        var activity = ActivitySource.StartActivity($"rabbitmq consume {args.RoutingKey}", ActivityKind.Consumer, parentContext.ActivityContext);
        if (activity == null)
        {
            return null;
        }

        activity.SetTag("messaging.system", "rabbitmq");
        activity.SetTag("messaging.destination", "ev.platform");
        activity.SetTag("messaging.destination_kind", "topic");
        activity.SetTag("messaging.rabbitmq.routing_key", args.RoutingKey);
        activity.SetTag("messaging.rabbitmq.queue", args.RoutingKey);

        return activity;
    }
}
