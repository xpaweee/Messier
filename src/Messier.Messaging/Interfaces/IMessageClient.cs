using Messier.Interfaces;

namespace Messier.Messaging.Interfaces;

public interface IMessageClient
{
    Task SendAsync<TType>(MessageWrapper<TType> message, CancellationToken cancellationToken = default)
        where TType : IMessage;
}