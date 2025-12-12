using Ev.Station.Infrastructure;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Ev.Shared.Messaging.RabbitMq;
using Ev.Station.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddApplicationPart(typeof(Program).Assembly);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(Ev.Station.Application.Mapping.StationProfile).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<Ev.Station.Application.Validation.CreateStationRequestValidator>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddMediatR(typeof(Ev.Station.Application.Services.IStationService).Assembly);

var connString = builder.Configuration.GetConnectionString("StationDb") ??
                 builder.Configuration["EV__POSTGRES__CONNECTIONSTRING"] ??
                 "Host=localhost;Port=5432;Database=station_db;Username=admin;Password=admin";

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<StationDbContext>();
    db.Database.Migrate();
}

app.MapControllers();
app.MapHealthChecks("/health/ready");
app.MapHealthChecks("/health/live");

app.Run();
