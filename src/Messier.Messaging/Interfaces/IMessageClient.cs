using Messier.Interfaces;

namespace Messier.Messaging.Interfaces;

public interface IMessageClient
{
    public Task SendAsync<TMessage>(TMessage message ,CancellationToken cancellationToken) where TMessage : IMessage;
}