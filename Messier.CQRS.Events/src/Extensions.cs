using Messier.CQRS.Events.Interfaces;
using Messier.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Messier.CQRS.Events;

public static class Extensions
{
    public static IMessierBuilder UseEventHandlers(this IMessierBuilder builder)
    {
        builder.ServiceCollection.Scan(x =>
        {
            x.FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
                .AddClasses(c => c.AssignableTo(typeof(IEventHandler<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime();
        });

        return builder;
    }

    public static IMessierBuilder AddEventHandlers(this IMessierBuilder builder)
    {
        builder.ServiceCollection.AddScoped<IEventDispatcher, EventDispatcher.EventDispatcher>();
        return builder;
    }
}