using Ev.Reservation.Infrastructure;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Ev.Shared.Messaging.RabbitMq;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(Ev.Reservation.Application.Mapping.ReservationProfile).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<Ev.Reservation.Application.Validation.CreateReservationRequestValidator>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddMediatR(typeof(Ev.Reservation.Application.Services.IReservationService).Assembly);

var connString = builder.Configuration.GetConnectionString("ReservationDb") ??
                 builder.Configuration["EV__POSTGRES__CONNECTIONSTRING"] ??
                 "Host=192.168.1.191;Port=5432;Database=reservation_db;Username=admin;Password=admin";

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
    .AddDbContextCheck<ReservationDbContext>("reservation-db");

var stationServiceBaseUrl = builder.Configuration["Services:StationServiceBaseUrl"]
                            ?? builder.Configuration["EV__SERVICES__STATION__BASEURL"]
                            ?? "http://localhost:5046";

builder.Services.AddReservationInfrastructure(connString, rabbitOptions, stationServiceBaseUrl);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ReservationDbContext>();
    db.Database.Migrate();
}

app.MapControllers();
app.MapHealthChecks("/health/ready");
app.MapHealthChecks("/health/live");

app.Run();
