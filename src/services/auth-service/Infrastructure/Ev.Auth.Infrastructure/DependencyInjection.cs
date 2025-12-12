using Ev.Auth.Application.Services;
using Ev.Auth.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Ev.Auth.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthInfrastructure(this IServiceCollection services, string issuer, string audience, string signingKey, TimeSpan tokenLifetime)
    {
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        services.AddScoped<IAuthService>(sp =>
        {
            var repo = sp.GetRequiredService<IUserRepository>();
            var mapper = sp.GetRequiredService<AutoMapper.IMapper>();
            return new AuthService(repo, mapper, issuer, audience, signingKey, tokenLifetime);
        });

        return services;
    }
}
