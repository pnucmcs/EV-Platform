using System.Collections.Generic;
using System.Text.Json;
using Ev.Pricing.Api.Configuration;
using Ev.Pricing.Application.Mapping;
using Ev.Pricing.Application.Services;
using Ev.Pricing.Application.Validation;
using Ev.Pricing.Infrastructure;
using Ev.Pricing.Infrastructure.Persistence;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

builder.Services.AddAutoMapper(typeof(PricingProfile).Assembly);
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<TariffPlanValidator>();

builder.Services.AddOptions<AppOptions>()
    .BindConfiguration("App")
    .ValidateOnStart();

builder.Services.AddOptions<DatabaseOptions>()
    .BindConfiguration("Database")
    .PostConfigure(opt => opt.ConnectionString ??= builder.Configuration.GetConnectionString("Postgres"))
    .ValidateOnStart();

builder.Services.AddPricingInfrastructure(builder.Configuration);

builder.Services.AddScoped<ITariffPlanService, TariffPlanService>();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<PricingDbContext>("pricing-db");

var appOptions = builder.Configuration.GetSection("App").Get<AppOptions>() ?? new AppOptions();
var otelSection = builder.Configuration.GetSection("OpenTelemetry");
var otelEnabled = otelSection.GetValue<bool?>("Enabled") ?? true;
if (otelEnabled)
{
    builder.Services.AddOpenTelemetry()
        .ConfigureResource(r => r.AddService(serviceName: "pricing-service", serviceVersion: appOptions.Version ?? "1.0.0")
            .AddAttributes(new[]
            {
                new KeyValuePair<string, object>("deployment.environment", appOptions.Environment ?? builder.Environment.EnvironmentName)
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
    var db = scope.ServiceProvider.GetRequiredService<PricingDbContext>();
    await db.Database.EnsureCreatedAsync();
}

app.MapControllers();
app.MapHealthChecks("/health/ready");
app.MapHealthChecks("/health/live");
app.MapPrometheusScrapingEndpoint("/metrics");

app.Run();
