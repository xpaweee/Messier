using Messier.CQRS.Queries.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Messier.CQRS.Queries.Dispatchers;

public class QueryDispatcher : IQueryDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public QueryDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
        var handler = scope.ServiceProvider.GetRequiredService(handlerType);

        return await (Task<TResult>)handlerType.GetMethod("HandleAsync")?.Invoke(handler, new[] { query });
    }
}