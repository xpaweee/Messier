
namespace Messier.Interfaces;

public interface IQuery : IMessage
{
    
}

public interface IQuery<TResult> : IQuery
{
    
}