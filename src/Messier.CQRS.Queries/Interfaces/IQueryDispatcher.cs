namespace Messier.CQRS.Queries.Interfaces;

public interface IQueryDispatcher
{
    Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
}