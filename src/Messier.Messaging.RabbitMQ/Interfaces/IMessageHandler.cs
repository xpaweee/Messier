using Messier.Interfaces;

namespace Messier.Messaging.RabbitMQ.Interfaces;

public interface IMessageHandler
{
    Task HandleAsync<TMessage>(Func<IServiceProvider, TMessage, CancellationToken, Task> handler, TMessage message,
        CancellationToken cancellationToken = default) where TMessage : IMessage;
}