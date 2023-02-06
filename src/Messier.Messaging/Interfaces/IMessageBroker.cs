using Messier.Interfaces;

namespace Messier.Messaging.Interfaces;

public interface IMessageBroker
{
    public Task SendAsync<TMessage>(TMessage message ,CancellationToken cancellationToken) where TMessage : IMessage;
}