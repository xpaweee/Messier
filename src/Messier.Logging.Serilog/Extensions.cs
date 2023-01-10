using Messier.Base;
using Messier.Interfaces;
using Messier.Logging.Serilog.Base;
using Messier.Logging.Serilog.Decorators;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Messier.Logging.Serilog;

public static class Extensions
{
    private const string _serilogSectionName = "serilog";
    private const string _appSectionName = "app";
    
    public static IServiceCollection AddLogger(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.Configure<SerilogOptions>(configuration.GetSection(_serilogSectionName));
        serviceCollection.TryDecorate(typeof(ICommandHandler<>), typeof(LoggingCommandHandlerDecorator<>));

        return serviceCollection;
    }
    
    public static WebApplicationBuilder AddMessierLogging(this WebApplicationBuilder builder,
        Action<LoggerConfiguration>? configure = null)
    {
        builder.Host.AddLogging(configure);
        
        return builder;
    }


    private static IHostBuilder AddLogging(this IHostBuilder builder,Action<LoggerConfiguration>? configure)
    {
        return builder.UseSerilog((context, loggerConfiguration) =>
        {
            var appOptions = context.Configuration.BindOptions<AppOptions>(_appSectionName);
            var serilogOptions = context.Configuration.BindOptions<SerilogOptions>(_serilogSectionName);

            loggerConfiguration.Enrich.FromLogContext()
                .MinimumLevel.Is(LogEventLevel.Information)
                .Enrich.WithProperty("Environment", context.HostingEnvironment)
                .Enrich.WithProperty("Application", appOptions.Name)
                .Enrich.WithProperty("Version", appOptions.Version);

            if (serilogOptions.Seq.Enabled)
            {
                loggerConfiguration.WriteTo.Seq(serilogOptions.Seq.Url, apiKey: serilogOptions.Seq.Apikey);
            }

            if (serilogOptions.Console.Enabled)
            {
                loggerConfiguration.WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {Message}{NewLine}{Exception}");
            }

            configure?.Invoke(loggerConfiguration);
        });
    }
}