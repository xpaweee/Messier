using System.Reflection;
using Messier.Messaging.Interfaces;
using Messier.Metrics.Prometheus.Attributes;
using Messier.Metrics.Prometheus.Decorators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;

namespace Messier.Metrics.Prometheus;

public static class Extension
{
    private static readonly string _metricsSectionName = "metrics";
    
    public static IServiceCollection AddMetrics(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(_metricsSectionName);
        var options = section.BindOptions<MetricsOptions>();
        services.Configure<MetricsOptions>(section);

        if (!options.Enabled)
        {
            return services;
        }

     
        services.AddOpenTelemetryMetrics(builder =>
            {
                // var attributes = GetMeterAttributes();
                // foreach (var attribute in attributes)
                // {
                //     builder.AddMeter(attribute.Key);
                // }
                
                builder.AddAspNetCoreInstrumentation();
                builder.AddHttpClientInstrumentation();
                builder.AddRuntimeInstrumentation();

                builder.AddPrometheusExporter(prometheus => { prometheus.ScrapeEndpointPath = options.Endpoint; });
            });

        services.TryDecorate<IMessageClient, MessageBrokerMetricsDecorator>();
        
        return services;
    }
    
    private static IEnumerable<MeterAttribute?> GetMeterAttributes()
        => AppDomain.CurrentDomain.GetAssemblies()
            .Where(x => !x.IsDynamic)
            .SelectMany(x => x.GetTypes())
            .Where(x => x.IsClass && x.GetCustomAttribute<MeterAttribute>() is not null)
            .Select(x => x.GetCustomAttribute<MeterAttribute>())
            .Where(x => x is not null);
}