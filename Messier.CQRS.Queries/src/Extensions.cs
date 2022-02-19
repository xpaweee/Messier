using Messier.CQRS.Queries.Interfaces;
using Messier.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Messier.CQRS.Queries;

public static class Extensions
{
    public static IMessierBuilder UseQueryHandlers(this IMessierBuilder builder)
    {
        // builder.ServiceCollection.Scan(x =>
        // {
        //     
        //     x.FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
        //         .AddClasses()
        //         
        // });

        return builder;
    }

    public static IMessierBuilder AddQueryHandlers(this IMessierBuilder builder)
    {
        builder.ServiceCollection.AddSingleton<IQueryDispatcher, QueryDispatcher.QueryDispatcher>();

        return builder;
    }
}
