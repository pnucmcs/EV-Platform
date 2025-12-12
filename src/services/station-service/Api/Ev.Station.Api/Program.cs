using Ev.Station.Infrastructure;
using FluentValidation;
using MediatR;
using Ev.Shared.Messaging.RabbitMq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddApplicationPart(typeof(Program).Assembly);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(Ev.Station.Application.Mapping.StationProfile).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<Ev.Station.Application.Validation.CreateStationRequestValidator>();
builder.Services.AddMediatR(typeof(Ev.Station.Application.Services.IStationService).Assembly);

var mongoSettings = new StationMongoSettings
{
    ConnectionString = builder.Configuration["EV__MONGO__CONNECTIONSTRING"] ?? "mongodb://localhost:27017",
    Database = builder.Configuration["EV__MONGO__DATABASE"] ?? "ev_platform",
    Collection = builder.Configuration["EV__MONGO__STATIONS_COLLECTION"] ?? "stations"
};
var redisConnection = builder.Configuration["EV__REDIS__CONNECTIONSTRING"] ?? "localhost:6379";
var rabbitSection = builder.Configuration.GetSection("RabbitMq");
builder.Services.AddRabbitMqPublisher(opt =>
{
    opt.HostName = rabbitSection.GetValue<string>("Host") ?? "localhost";
    opt.Port = rabbitSection.GetValue<int?>("Port") ?? 5672;
    opt.UserName = rabbitSection.GetValue<string>("UserName") ?? "guest";
    opt.Password = rabbitSection.GetValue<string>("Password") ?? "guest";
    opt.Exchange = rabbitSection.GetValue<string>("Exchange") ?? "ev.events";
});

builder.Services.AddStationInfrastructure(
    mongoSettings,
    redisConnection,
    new RabbitMqOptions
    {
        HostName = rabbitSection.GetValue<string>("Host") ?? "localhost",
        Port = rabbitSection.GetValue<int?>("Port") ?? 5672,
        UserName = rabbitSection.GetValue<string>("UserName") ?? "guest",
        Password = rabbitSection.GetValue<string>("Password") ?? "guest",
        Exchange = rabbitSection.GetValue<string>("Exchange") ?? "ev.events"
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
