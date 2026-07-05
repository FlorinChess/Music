using Microsoft.Extensions.DependencyInjection;
using Music.WPF.Store;
using Serilog;

namespace Music.WPF;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<ILogger>(_ => new LoggerConfiguration().CreateLogger());

        // Add Stores as Singletons
        services.AddSingleton<TrackStore>();
        services.AddSingleton<NavigationStore>();
        services.AddSingleton<ModalNavigationStore>();

        return services;
    }
}
