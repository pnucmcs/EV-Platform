using System.Collections.Generic;
using Ev.Shared.Messaging.RabbitMq;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Notification.Api.Consumers;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var rabbitSection = builder.Configuration.GetSection("RabbitMq");
builder.Services.AddRabbitMqConsumer(opt =>
{
    opt.HostName = rabbitSection.GetValue<string>("Host") ?? "localhost";
    opt.Port = rabbitSection.GetValue<int?>("Port") ?? 5672;
    opt.UserName = rabbitSection.GetValue<string>("Username") ?? "guest";
    opt.Password = rabbitSection.GetValue<string>("Password") ?? "guest";
    opt.Exchange = rabbitSection.GetValue<string>("Exchange") ?? "ev.platform";
}, queueName: "notification-service.events",
    "station.*.v1",
    "reservation.*.v1",
    "session.*.v1");

builder.Services.AddSingleton<IRabbitMqMessageHandler, NotificationEventHandler>();

var otelSection = builder.Configuration.GetSection("OpenTelemetry");
var otelEnabled = otelSection.GetValue<bool?>("Enabled") ?? true;
if (otelEnabled)
{
    builder.Services.AddOpenTelemetry()
        .ConfigureResource(r => r.AddService(serviceName: "notification-service", serviceVersion: "1.0.0")
            .AddAttributes(new[]
            {
                new KeyValuePair<string, object>("deployment.environment", builder.Environment.EnvironmentName)
            }))
        .WithTracing(tracerProviderBuilder =>
        {
            tracerProviderBuilder
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddSource(RabbitMqTelemetry.ActivitySourceName);

            var exporter = otelSection.GetValue<string>("Exporter") ?? "otlp";
            if (exporter.Equals("otlp", StringComparison.OrdinalIgnoreCase))
            {
                var endpoint = otelSection.GetSection("Otlp").GetValue<string>("Endpoint") ?? "http://localhost:4317";
                tracerProviderBuilder.AddOtlpExporter(o => o.Endpoint = new Uri(endpoint));
            }
        });
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");

app.Run();
