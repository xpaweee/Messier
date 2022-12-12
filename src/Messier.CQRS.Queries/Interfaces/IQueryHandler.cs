namespace Messier.CQRS.Queries.Interfaces;

public interface IQueryHandler<in TQuery, TResult> where TQuery : class, IQuery
{
    Task<TResult> HadnleAsync(TQuery query, CancellationToken cancellationToken = default);
}