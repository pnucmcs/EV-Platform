using System.Collections.Generic;
using System.Text.Json;
using Ev.Billing.Api.Configuration;
using Ev.Billing.Api.Messaging;
using Ev.Billing.Api.Services;
using Ev.Billing.Application.Mapping;
using Ev.Billing.Application.Services;
using Ev.Billing.Application.Validation;
using Ev.Billing.Infrastructure;
using Ev.Billing.Infrastructure.Persistence;
using Ev.Platform.Contracts;
using Ev.Platform.Contracts.Events;
using Ev.Shared.Messaging.RabbitMq;
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

builder.Services.AddAutoMapper(typeof(BillingProfile).Assembly);
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateInvoiceValidator>();

builder.Services.AddOptions<AppOptions>()
    .BindConfiguration("App")
    .ValidateOnStart();

builder.Services.AddOptions<DatabaseOptions>()
    .BindConfiguration("Database")
    .PostConfigure(opt => opt.ConnectionString ??= builder.Configuration.GetConnectionString("Postgres"))
    .ValidateOnStart();

builder.Services.AddOptions<ServiceEndpointsOptions>()
    .BindConfiguration("Services")
    .ValidateOnStart();

builder.Services.AddBillingInfrastructure(builder.Configuration);
builder.Services.AddScoped<IInvoiceService, InvoiceService>();

var servicesOptions = builder.Configuration.GetSection("Services").Get<ServiceEndpointsOptions>() ?? new ServiceEndpointsOptions();
builder.Services.AddHttpClient<IPricingClient, PricingClient>(client =>
{
    if (!string.IsNullOrWhiteSpace(servicesOptions.PricingService.BaseUrl))
    {
        client.BaseAddress = new Uri(servicesOptions.PricingService.BaseUrl);
    }
});

builder.Services.AddHealthChecks()
    .AddDbContextCheck<BillingDbContext>("billing-db");

var rabbitSection = builder.Configuration.GetSection("RabbitMq");
builder.Services.AddRabbitMqConsumer(opt =>
{
    opt.HostName = rabbitSection.GetValue<string>("Host") ?? "localhost";
    opt.Port = rabbitSection.GetValue<int?>("Port") ?? 5672;
    opt.UserName = rabbitSection.GetValue<string>("Username") ?? "guest";
    opt.Password = rabbitSection.GetValue<string>("Password") ?? "guest";
    opt.Exchange = rabbitSection.GetValue<string>("Exchange") ?? "ev.platform";
}, queueName: "billing-service.events", EventRoutingKeys.ChargingSessionCompletedV1);
builder.Services.AddSingleton<IRabbitMqMessageHandler, SessionCompletedEventHandler>();

var appOptions = builder.Configuration.GetSection("App").Get<AppOptions>() ?? new AppOptions();
var otelSection = builder.Configuration.GetSection("OpenTelemetry");
var otelEnabled = otelSection.GetValue<bool?>("Enabled") ?? true;
if (otelEnabled)
{
    builder.Services.AddOpenTelemetry()
        .ConfigureResource(r => r.AddService(serviceName: "billing-service", serviceVersion: appOptions.Version ?? "1.0.0")
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
    var db = scope.ServiceProvider.GetRequiredService<BillingDbContext>();
    await db.Database.EnsureCreatedAsync();
}

app.MapControllers();
app.MapHealthChecks("/health/ready");
app.MapHealthChecks("/health/live");
app.MapPrometheusScrapingEndpoint("/metrics");

app.Run();
