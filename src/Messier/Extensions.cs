using Messier.Interfaces;
using Messier.Time;
using Microsoft.Extensions.DependencyInjection;

namespace Messier;

public static class Extensions
{
    public static IServiceCollection AddMessier(this IServiceCollection services)
    {
        services.AddSingleton<IClock, Clock>();

        return services;
    }
}