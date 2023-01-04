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
    private readonly IMessageTypeRegistry _messageTypeRegistry;
    
    public RabbitmqSubscriber(IBus bus, IMessageHandler messageHandler, IMessageTypeRegistry messageTypeRegistry)
    {
        _bus = bus;
        _messageHandler = messageHandler;
        _messageTypeRegistry = messageTypeRegistry;
    }
    
    public IMessageSubscriber Command<TCommand>() where TCommand : class, ICommand
        => Message<TCommand>((serviceProvider, command, cancellationToken) =>
            serviceProvider.GetRequiredService<IDispatcher>().SendAsync(command, cancellationToken));

    public IMessageSubscriber Message<TCommand>(Func<IServiceProvider, TCommand, CancellationToken, Task> handler) where TCommand : class, IMessage
    {
        _messageTypeRegistry.Register<TCommand>();
        var messageAttribute = typeof(TCommand).GetCustomAttribute<MessageAttribute>() ?? new MessageAttribute();

        _bus.PubSub.SubscribeAsync<TCommand>(messageAttribute.SubscriptionId,
            (message, cancellationToken) => _messageHandler.HandleAsync(handler, message, cancellationToken),
            configuration =>
            {
                if (!string.IsNullOrWhiteSpace(messageAttribute.Topic))
                {
                    configuration.WithTopic(messageAttribute.Topic);
                }

                if (!string.IsNullOrWhiteSpace(messageAttribute.Queue))
                {
                    configuration.WithQueueName(messageAttribute.Queue);
                }
            });

        return this;
    }
}