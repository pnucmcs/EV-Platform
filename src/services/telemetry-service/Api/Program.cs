using System.Collections.Generic;
using System.Text.Json;
using Ev.Telemetry.Application.Mapping;
using Ev.Telemetry.Application.Services;
using Ev.Telemetry.Application.Validation;
using Ev.Telemetry.Infrastructure;
using Ev.Telemetry.Infrastructure.Persistence;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.Configure(context.Configuration.GetSection("Kestrel"));
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(TelemetryProfile).Assembly);
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<TelemetryReadingValidator>();

builder.Services.AddTelemetryInfrastructure(builder.Configuration);
builder.Services.AddScoped<ITelemetryService, TelemetryService>();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<TelemetryDbContext>("telemetry-db");

var otelSection = builder.Configuration.GetSection("OpenTelemetry");
var otelEnabled = otelSection.GetValue<bool?>("Enabled") ?? true;
if (otelEnabled)
{
    builder.Services.AddOpenTelemetry()
        .ConfigureResource(r => r.AddService(serviceName: "telemetry-service", serviceVersion: "1.0.0")
            .AddAttributes(new[]
            {
                new KeyValuePair<string, object>("deployment.environment", builder.Environment.EnvironmentName)
            }))
        .WithTracing(tp =>
        {
            tp.AddAspNetCoreInstrumentation()
              .AddHttpClientInstrumentation()
              .AddEntityFrameworkCoreInstrumentation();

            var exporter = otelSection.GetValue<string>("Exporter") ?? "otlp";
            if (exporter.Equals("otlp", StringComparison.OrdinalIgnoreCase))
            {
                var endpoint = otelSection.GetSection("Otlp").GetValue<string>("Endpoint") ?? "http://localhost:4317";
                tp.AddOtlpExporter(o => o.Endpoint = new Uri(endpoint));
            }
        })
        .WithMetrics(mp =>
        {
            mp.AddAspNetCoreInstrumentation()
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
                ["traceId"] = context.HttpContext.TraceIdentifier
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

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
    var db = scope.ServiceProvider.GetRequiredService<TelemetryDbContext>();
    await db.Database.EnsureCreatedAsync();
}

app.MapControllers();
app.MapHealthChecks("/health/ready");
app.MapHealthChecks("/health/live");
app.MapPrometheusScrapingEndpoint("/metrics");

app.Run();
