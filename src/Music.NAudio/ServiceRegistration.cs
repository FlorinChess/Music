using Microsoft.Extensions.DependencyInjection;

namespace Music.NAudio;

public static class ServiceRegistration
{
    public static IServiceCollection AddNAudio(this IServiceCollection services)
    {
        services.AddSingleton<WaveformGenerator.WaveformGenerator>();

        return services;
    }
}
