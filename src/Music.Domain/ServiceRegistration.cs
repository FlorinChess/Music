using Microsoft.Extensions.DependencyInjection;

namespace Music.Domain;

public static class ServiceRegistration
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddSingleton<PlaylistPersistenceService>();

        return services;
    }
}
