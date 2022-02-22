using Messier.CQRS.Queries.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Messier.CQRS.Queries.QueryDispatcher;

internal sealed class QueryDispatcher : IQueryDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public QueryDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResult> DispatchAsync<TQuery, TResult>(TQuery query) where TResult : class where TQuery : class, IQuery
    {
        using var scope = _serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IQueryHandler<TQuery,TResult>>();
        
        return await handler.HandleQueryAsync(query);
    }
}