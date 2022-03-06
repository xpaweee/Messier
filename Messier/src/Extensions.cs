using Messier.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messier;

public static class Extensions
{
    public static IMessierBuilder UseMessier(this IServiceCollection serviceCollection)
    {
        MessierBuilder messierBuilder = new(serviceCollection);
        
        
        return messierBuilder;
    }
    public static IApplicationBuilder AddMessier(this IApplicationBuilder applicationBuilder)
    {
        return applicationBuilder;
    }
    
    public static TModel GetOptions<TModel>(this IMessierBuilder builder, string settingsSectionName)
        where TModel : new()
    {
        using var serviceProvider = builder.ServiceCollection.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        return configuration.GetOptions<TModel>(settingsSectionName);
    }
    
    public static TModel GetOptions<TModel>(this IConfiguration configuration, string sectionName)
        where TModel : new()
    {
        var model = new TModel();
        configuration.GetSection(sectionName).Bind(model);
        return model;
    }

    public static ServiceProvider BuildServiceProvider(this IServiceCollection serviceCollection)
    {
        var serviceProvider = serviceCollection.BuildServiceProvider();
        
        return serviceProvider;
    }
    
}