using Microsoft.Extensions.DependencyInjection;
using Music.APIs.Spotify;

namespace Music.APIs;

public static class ServiceRegistration
{
    public static IServiceCollection AddAPIs(this IServiceCollection services)
    {
        services.AddHttpClient();

        services.AddSingleton<SpotifyService>();
        services.AddTransient<MusicMetadataService>();

        return services;
    }
}
