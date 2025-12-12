using Ev.Reservation.Application.Services;
using Ev.Reservation.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ev.Shared.Messaging.RabbitMq;

namespace Ev.Reservation.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddReservationInfrastructure(this IServiceCollection services, string postgresConnection, RabbitMqOptions rabbitOptions)
    {
        services.AddDbContext<ReservationDbContext>(opt => opt.UseNpgsql(postgresConnection));
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IReservationService, ReservationService>();
        services.AddSingleton(rabbitOptions);
        return services;
    }
}
