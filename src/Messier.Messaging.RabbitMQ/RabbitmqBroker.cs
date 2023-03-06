using EasyNetQ;
using Messier.Messaging.Interfaces;
using Microsoft.Extensions.Logging;
using IMessage = Messier.Interfaces.IMessage;

namespace Messier.Messaging.RabbitMQ;

internal sealed class RabbitmqBroker : IMessageClient
{
    private readonly IBus _bus;
    private readonly ILogger<RabbitmqBroker> _logger;

    public RabbitmqBroker(IBus bus, ILogger<RabbitmqBroker> logger)
    {
        _bus = bus;
        _logger = logger;
    }
    
    public async Task SendAsync<TType>(MessageWrapper<TType> message, CancellationToken cancellationToken = default) where TType : IMessage
    {
        _logger.LogInformation($"Sending a message: {message.GetType().Name}");
        await _bus.PubSub.PublishAsync(message.Message, cancellationToken);
    }
}