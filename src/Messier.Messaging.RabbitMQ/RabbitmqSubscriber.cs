using EasyNetQ;
using Messier.Interfaces;
using Messier.Messaging.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using IMessage = Messier.Interfaces.IMessage;

namespace Messier.Messaging.RabbitMQ;

internal sealed class RabbitmqSubscriber : IMessageSubscriber
{
    private readonly IBus _bus;

    public RabbitmqSubscriber(IBus bus)
    {
        _bus = bus;
    }
    
    public IMessageSubscriber Command<TCommand>() where TCommand : class, ICommand
        => Message<TCommand>((serviceProvider, command, cancellationToken) =>
            serviceProvider.GetRequiredService<IDispatcher>().SendAsync(command, cancellationToken));

    public IMessageSubscriber Message<TCommand>(Func<IServiceProvider, TCommand, CancellationToken, Task> handler) where TCommand : class, IMessage
    {
        //_bus.PubSub.SubscribeAsync()
    }
}