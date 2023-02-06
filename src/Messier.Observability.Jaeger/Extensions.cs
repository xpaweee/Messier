using System.ComponentModel.Design;
using Messier.Base;
using Messier.Messaging.Interfaces;
using Messier.Messaging.RabbitMQ.Interfaces;
using Messier.Observability.Jaeger.Base;
using Messier.Observability.Jaeger.Decorators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;


namespace Messier.Observability.Jaeger;

public static class Extensions
{
    private const string _tracingSectionName = "tracing";
    
    public static IServiceCollection AddTracing(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var section = configuration.GetSection(_tracingSectionName);
        var options = section.BindOptions<TracingOptions>();
        serviceCollection.Configure<TracingOptions>(section);

        if (!options.Enabled)
        {
            return serviceCollection;
        }
        
        var appOptions = configuration.BindOptions<AppOptions>("app");

        if (string.IsNullOrWhiteSpace(appOptions.Name))
        {
            throw new ArgumentNullException("Application name is null.");
        }

        serviceCollection.AddOpenTelemetryTracing(builder =>
        {
            builder.SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddTelemetrySdk()
                    .AddEnvironmentVariableDetector()
                    .AddService(appOptions.Name))
                .AddSource(appOptions.Name)
                .AddSource(MessageBrokerTracingDecorator.ActivitySourceName)
                .AddSource(MessageHandlerTracingDecorator.ActivitySourceName)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddSqlClientInstrumentation();

            // builder.AddConsoleExporter();
            builder.AddJaegerExporter(configure =>
            {
                configure.AgentHost = options.Jaeger.AgentHost;
                configure.AgentPort = options.Jaeger.AgentPort;
                configure.MaxPayloadSizeInBytes = 4028;
                configure.ExportProcessorType = ExportProcessorType.Batch;
            });
        });

        serviceCollection.TryDecorate<IMessageBroker, MessageBrokerTracingDecorator>();
        serviceCollection.TryDecorate<IMessageHandler, MessageHandlerTracingDecorator>();

        return serviceCollection;
    }
}