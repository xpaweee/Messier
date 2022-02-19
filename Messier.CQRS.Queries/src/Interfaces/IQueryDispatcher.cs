namespace Messier.CQRS.Queries.Interfaces;

public interface IQueryDispatcher
{
    Task<TResult> DispatchAsync<TResult, TQuery>(TQuery query)  where TQuery : class, IQuery
                                                                where TResult : class;
}