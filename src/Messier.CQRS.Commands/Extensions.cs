using System.Reflection;
using Messier.CQRS.Commands.Dispatchers;
using Messier.CQRS.Commands.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Messier.CQRS.Commands;

public static class Extensions
{
    public static IServiceCollection AddCommands(this IServiceCollection services)
    {
        var assembly = Assembly.GetCallingAssembly();
        services.AddSingleton<ICommandDispatcher, CommandDispatcher>();
        services.Scan(x => x.FromAssemblies(assembly)
            .AddClasses(y => y.AssignableTo(typeof(ICommandHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
            
        return services;
    }
}