using System.Reflection;
using Messier.CQRS.Queries.Dispatchers;
using Messier.CQRS.Queries.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Messier.CQRS.Queries;

public static class Extensions
{
    public static IServiceCollection AddQueries(this IServiceCollection services)
    {
        var assembly = Assembly.GetCallingAssembly();
            
        services.AddSingleton<IQueryDispatcher, QueryDispatcher>();
        services.Scan(x => x.FromAssemblies(assembly)
            .AddClasses(y => y.AssignableTo(typeof(IQueryHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
            
        return services;
    }
}