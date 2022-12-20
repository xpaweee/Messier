using System.Reflection;
using EasyNetQ;
using Messier.Attributes;
using Messier.Interfaces;
using Messier.Messaging.Interfaces;
using Messier.Messaging.RabbitMQ.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using IMessage = Messier.Interfaces.IMessage;

namespace Messier.Messaging.RabbitMQ;

internal sealed class RabbitmqSubscriber : IMessageSubscriber
{
    private readonly IBus _bus;
    private readonly IMessageHandler _messageHandler;
    
    public RabbitmqSubscriber(IBus bus, IMessageHandler messageHandler)
    {
        _bus = bus;
        _messageHandler = messageHandler;
    }
    
    public IMessageSubscriber Command<TCommand>() where TCommand : class, ICommand
        => Message<TCommand>((serviceProvider, command, cancellationToken) =>
            serviceProvider.GetRequiredService<IDispatcher>().SendAsync(command, cancellationToken));

    public IMessageSubscriber Message<TCommand>(Func<IServiceProvider, TCommand, CancellationToken, Task> handler) where TCommand : class, IMessage
    {
        var messageDetails = typeof(TCommand).GetCustomAttribute<MessageAttribute>();

        _bus.PubSub.SubscribeAsync<TCommand>(messageDetails.SubscriptionId, (message, cancellationToken) =>
        {
            return _messageHandler.HandleAsync(handler, message, cancellationToken);
        }, null);

        return this;
    }
}