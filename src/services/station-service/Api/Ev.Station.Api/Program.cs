using Ev.Station.Infrastructure;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Ev.Shared.Messaging.RabbitMq;
using Ev.Station.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;
using Ev.Station.Api.Middleware;
using Ev.Station.Api.Configuration;
using Ev.Station.Infrastructure.Outbox;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.Configure(context.Configuration.GetSection("Kestrel"));
});

builder.Services.AddControllers()
    .AddApplicationPart(typeof(Program).Assembly);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(Ev.Station.Application.Mapping.StationProfile).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<Ev.Station.Application.Validation.CreateStationRequestValidator>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddMediatR(typeof(Ev.Station.Application.Services.IStationService).Assembly);

builder.Services.AddOptions<AppOptions>()
    .BindConfiguration("App")
    .ValidateOnStart();

builder.Services.AddOptions<DatabaseOptions>()
    .BindConfiguration("Database")
    .PostConfigure(options =>
    {
        options.ConnectionString ??= builder.Configuration.GetConnectionString("Postgres");
    })
    .ValidateOnStart();

builder.Services.Configure<OutboxOptions>(builder.Configuration.GetSection("Outbox"));

var dbOptions = builder.Configuration.GetSection("Database").Get<DatabaseOptions>() ?? new DatabaseOptions();
dbOptions.ConnectionString ??= builder.Configuration.GetConnectionString("Postgres");
var connString = dbOptions.ConnectionString ?? throw new InvalidOperationException("ConnectionStrings:Postgres must be configured.");

var rabbitSection = builder.Configuration.GetSection("RabbitMq");
var rabbitOptions = new RabbitMqOptions
{
    HostName = rabbitSection.GetValue<string>("Host") ?? "localhost",
    Port = rabbitSection.GetValue<int?>("Port") ?? 5672,
    UserName = rabbitSection.GetValue<string>("UserName") ?? "guest",
    Password = rabbitSection.GetValue<string>("Password") ?? "guest",
    Exchange = rabbitSection.GetValue<string>("Exchange") ?? "ev.events"
};

builder.Services.AddRabbitMqPublisher(opt =>
{
    opt.HostName = rabbitSection.GetValue<string>("Host") ?? "localhost";
    opt.Port = rabbitSection.GetValue<int?>("Port") ?? 5672;
    opt.UserName = rabbitSection.GetValue<string>("UserName") ?? "guest";
    opt.Password = rabbitSection.GetValue<string>("Password") ?? "guest";
    opt.Exchange = rabbitSection.GetValue<string>("Exchange") ?? "ev.events";
});

builder.Services.AddHealthChecks()
    .AddDbContextCheck<StationDbContext>("station-db");

builder.Services.AddStationInfrastructure(connString, rabbitOptions);
builder.Services.AddHostedService<OutboxDispatcherHostedService>();

var appOptions = builder.Configuration.GetSection("App").Get<AppOptions>() ?? new AppOptions();
var otelSection = builder.Configuration.GetSection("OpenTelemetry");
var otelEnabled = otelSection.GetValue<bool?>("Enabled") ?? true;
if (otelEnabled)
{
    builder.Services.AddOpenTelemetry()
        .ConfigureResource(r => r.AddService(serviceName: "station-service", serviceVersion: appOptions.Version ?? "1.0.0")
            .AddAttributes(new[]
            {
                new KeyValuePair<string, object>("deployment.environment", appOptions.Environment ?? "Production")
            }))
        .WithTracing(tracerProviderBuilder =>
        {
            tracerProviderBuilder
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddEntityFrameworkCoreInstrumentation()
                .AddSource(RabbitMqTelemetry.ActivitySourceName);

            var exporter = otelSection.GetValue<string>("Exporter") ?? "otlp";
            if (exporter.Equals("otlp", StringComparison.OrdinalIgnoreCase))
            {
                var endpoint = otelSection.GetSection("Otlp").GetValue<string>("Endpoint") ?? "http://localhost:4317";
                tracerProviderBuilder.AddOtlpExporter(o => o.Endpoint = new Uri(endpoint));
            }
        })
        .WithMetrics(metricsBuilder =>
        {
            metricsBuilder.AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddPrometheusExporter();
        });
}

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var problem = new ValidationProblemDetails(context.ModelState)
        {
            Status = StatusCodes.Status400BadRequest,
            Instance = context.HttpContext.Request.Path,
            Extensions =
            {
                ["traceId"] = context.HttpContext.TraceIdentifier,
                ["correlationId"] = context.HttpContext.Response.Headers["X-Correlation-ID"].ToString()
            }
        };
        return new BadRequestObjectResult(problem);
    };
});

builder.Host.UseSerilog((ctx, services, cfg) =>
{
    cfg.ReadFrom.Configuration(ctx.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseMiddleware<CorrelationIdMiddleware>();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";

        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        var problem = new ProblemDetails
        {
            Title = "An unexpected error occurred.",
            Status = StatusCodes.Status500InternalServerError,
            Instance = context.Request.Path,
        };
        problem.Extensions["traceId"] = context.TraceIdentifier;
        problem.Extensions["correlationId"] = context.Response.Headers["X-Correlation-ID"].ToString();
        if (app.Environment.IsDevelopment() && exception is not null)
        {
            problem.Detail = exception.Message;
        }

        await JsonSerializer.SerializeAsync(context.Response.Body, problem);
    });
});

app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<StationDbContext>();
    db.Database.Migrate();
}

app.MapControllers();
app.MapHealthChecks("/health/ready");
app.MapHealthChecks("/health/live");
app.MapPrometheusScrapingEndpoint("/metrics");

app.Run();
