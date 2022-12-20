using Messier.Interfaces;
using Messier.Time;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messier;

public static class Extensions
{
    public static IServiceCollection AddMessier(this IServiceCollection services)
    {
        services.AddSingleton<IClock, Clock>();

        return services;
    }
    
    public static T BindOptions<T>(this IConfiguration configuration, string sectionName) where T : new()
        => BindOptions<T>(configuration.GetSection(sectionName));

    public static T BindOptions<T>(this IConfigurationSection section) where T : new()
    {
        var options = new T();
        section.Bind(options);
        return options;
    }
}