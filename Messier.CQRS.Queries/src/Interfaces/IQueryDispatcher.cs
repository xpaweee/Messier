namespace Messier.CQRS.Queries.Interfaces;

public interface IQueryDispatcher
{
    Task<TResult> DispatchAsync<TQuery, TResult>(TQuery query)  where TQuery : class, IQuery
                                                                where TResult : class;
}