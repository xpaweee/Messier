namespace Messier.CQRS.Queries.Interfaces;

public interface IQueryHandler<in TQuery,  TResult> where TQuery : class, IQuery 
                                                    where TResult: class
{
    Task<TResult> HandleQueryAsync(TQuery query);
}