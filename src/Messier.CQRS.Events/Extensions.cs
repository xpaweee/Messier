using System.Reflection;
using Messier.CQRS.Events.Dispatcher;
using Messier.CQRS.Events.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Messier.CQRS.Events;

public static class Extensions
{
    public static IServiceCollection AddEvents(this IServiceCollection services)
    {
        var assembly = Assembly.GetCallingAssembly();
        services.AddSingleton<IEventDispatcher, EventDispatcher>();
        services.Scan(x => x.FromAssemblies(assembly)
            .AddClasses(y => y.AssignableTo(typeof(IEventHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
            
        return services;
    }
}