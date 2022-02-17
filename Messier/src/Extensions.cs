using Messier.Interfaces;
using Microsoft.AspNetCore.Builder;
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
    
}