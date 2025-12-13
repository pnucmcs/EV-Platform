using System.Collections.Generic;
using System.Text.Json;
using Ev.Notification.Application.Mapping;
using Ev.Notification.Application.Services;
using Ev.Notification.Infrastructure;
using Ev.Notification.Infrastructure.Persistence;
using Ev.Platform.Contracts;
using Ev.Platform.Contracts.Events;
using Ev.Shared.Messaging.RabbitMq;
using FluentValidation;
using FluentValidation.AspNetCore;
using Notification.Api.Consumers;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(NotificationProfile).Assembly);
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<NotificationProfile>();
builder.Services.AddNotificationInfrastructure(builder.Configuration);
builder.Services.AddScoped<INotificationService, NotificationService>();

var rabbitSection = builder.Configuration.GetSection("RabbitMq");
builder.Services.AddRabbitMqConsumer(opt =>
{
    opt.HostName = rabbitSection.GetValue<string>("Host") ?? "localhost";
    opt.Port = rabbitSection.GetValue<int?>("Port") ?? 5672;
    opt.UserName = rabbitSection.GetValue<string>("Username") ?? "guest";
    opt.Password = rabbitSection.GetValue<string>("Password") ?? "guest";
    opt.Exchange = rabbitSection.GetValue<string>("Exchange") ?? "ev.platform";
}, queueName: "notification-service.events",
    EventRoutingKeys.StationCreatedV1,
    EventRoutingKeys.ReservationCreatedV1,
    EventRoutingKeys.ChargingSessionStartedV1,
    EventRoutingKeys.ChargingSessionCompletedV1);

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

builder.Services.AddHealthChecks()
    .AddDbContextCheck<NotificationDbContext>("notification-db");

builder.Host.UseSerilog((ctx, services, cfg) =>
{
    cfg.ReadFrom.Configuration(ctx.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
    await db.Database.EnsureCreatedAsync();
}

app.MapControllers();
app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");
app.MapPrometheusScrapingEndpoint("/metrics");

app.Run();
