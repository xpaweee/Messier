using Messier.Api;
using Messier.Base;
using Messier.Interfaces;
using Messier.Time;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messier;

public static class Extensions
{
    private const string _appSectionName = "app";
    public static IServiceCollection AddMessier(this IServiceCollection services, IConfiguration configuration)
    {
        var appSection = configuration.GetSection(_appSectionName);
        var appOptions = appSection.BindOptions<AppOptions>();
        
        services.AddSingleton<IClock, Clock>();
        services.AddHttpContextAccessor();
        services.AddSingleton<IContextProvider, ContextProvider>();
        services.AddSingleton<IContextAccessor, ContextAccessor>();
        services.AddSingleton<IJsonSerializer, JsonSerializer.JsonSerializer>();
        
        services.Configure<AppOptions>(appSection);
        services.AddSingleton(appOptions);
        
        Console.WriteLine(Figgle.FiggleFonts.Standard.Render($"{appOptions.Name} {appOptions.Version}"));
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
    
    private const string CorrelationIdKey = "correlation-id";
    public static string? GetCorrelationId(this HttpContext context)
        => context.Items.TryGetValue(CorrelationIdKey, out var correlationId) ? correlationId?.ToString(): null;
}