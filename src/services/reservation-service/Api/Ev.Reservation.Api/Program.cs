using Ev.Reservation.Infrastructure;
using FluentValidation;
using MediatR;
using Ev.Shared.Messaging.RabbitMq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connString = builder.Configuration.GetConnectionString("ReservationDb") ??
                 builder.Configuration["EV__POSTGRES__CONNECTIONSTRING"] ??
                 "Host=localhost;Port=5432;Database=reservation_db;Username=admin;Password=admin";

var rabbitSection = builder.Configuration.GetSection("RabbitMq");
var rabbitOptions = new RabbitMqOptions
{
    HostName = rabbitSection.GetValue<string>("Host") ?? "localhost",
    Port = rabbitSection.GetValue<int?>("Port") ?? 5672,
    UserName = rabbitSection.GetValue<string>("UserName") ?? "guest",
    Password = rabbitSection.GetValue<string>("Password") ?? "guest",
    Exchange = rabbitSection.GetValue<string>("Exchange") ?? "ev.events"
};

builder.Services.AddAutoMapper(typeof(Ev.Reservation.Application.Mapping.ReservationProfile).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<Ev.Reservation.Application.Validation.CreateReservationRequestValidator>();
builder.Services.AddMediatR(typeof(Ev.Reservation.Application.Services.IReservationService).Assembly);
builder.Services.AddRabbitMqPublisher(opt =>
{
    opt.HostName = rabbitSection.GetValue<string>("Host") ?? "localhost";
    opt.Port = rabbitSection.GetValue<int?>("Port") ?? 5672;
    opt.UserName = rabbitSection.GetValue<string>("UserName") ?? "guest";
    opt.Password = rabbitSection.GetValue<string>("Password") ?? "guest";
    opt.Exchange = rabbitSection.GetValue<string>("Exchange") ?? "ev.events";
});
builder.Services.AddReservationInfrastructure(connString, rabbitOptions);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
