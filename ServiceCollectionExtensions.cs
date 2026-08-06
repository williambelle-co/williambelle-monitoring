using Microsoft.Extensions.DependencyInjection;

namespace WilliamBelle.Monitoring;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds monitoring to an ASP.NET Core application:
    /// <code>
    /// builder.Services.AddWilliamBelleMonitoring(o =>
    /// {
    ///     o.IngestUrl = builder.Configuration["Monitoring:IngestUrl"]!;
    ///     o.AppId = builder.Configuration["Monitoring:AppId"]!;
    ///     o.SigningKey = builder.Configuration["Monitoring:SigningKey"]!;
    /// });
    /// </code>
    /// </summary>
    public static IServiceCollection AddWilliamBelleMonitoring(
        this IServiceCollection services, Action<MonitoringOptions> configure)
    {
        services.Configure(configure);
        services.AddHttpClient("bellwether-sensor");
        services.AddHostedService<SensorService>();
        return services;
    }
}
