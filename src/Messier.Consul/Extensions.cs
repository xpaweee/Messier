using Consul;
using Messier.Consul.Base;
using Messier.Consul.HttpHandler;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messier.Consul;

public static class Extensions
{
    private static string _sectionName = "consul";
    
    public static IServiceCollection AddConsul(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var section = configuration.GetSection("consul");
        var options = section.BindOptions<ConsulOptions>();
        serviceCollection.Configure<ConsulOptions>(section);
        
        if (!options.Enabled)
        {
            return serviceCollection;
        }
        
        serviceCollection.AddTransient<ConsulHttpHandler>();
        serviceCollection.AddHostedService<ConsulRegistrationBackgroundService>();
        serviceCollection.AddSingleton<IConsulClient>(new ConsulClient(consulConfig =>
        {
            consulConfig.Address = new Uri(options.Url);
        }));
        
        return serviceCollection;
    }
    
    public static IHttpClientBuilder AddConsulHandler(this IHttpClientBuilder builder)
        => builder.AddHttpMessageHandler<ConsulHttpHandler>();
}