using Messier.CQRS.Commands.Interfaces;
using Messier.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Messier.CQRS.Commands;

public static class Extensions
{
    public static IMessierBuilder UseCommandHandlers(this IMessierBuilder messierBuilder)
    {
        messierBuilder.ServiceCollection.Scan(s =>
        {
            s.FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
                .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime();
        });

        return messierBuilder;
    }

    public static IMessierBuilder AddCommandHandlers(this IMessierBuilder messierBuilder)
    {
        messierBuilder.ServiceCollection.AddSingleton<ICommandDispatcher, CommandDispatcher.CommandDispatcher>();

        return messierBuilder;
    }
}