using EasyNetQ;
using Messier.Messaging.Interfaces;
using Microsoft.Extensions.Logging;
using IMessage = Messier.Interfaces.IMessage;

namespace Messier.Messaging.RabbitMQ;

internal sealed class RabbitmqClient : IMessageClient
{
    private readonly IBus _bus;
    private readonly ILogger<RabbitmqClient> _logger;

    public RabbitmqClient(IBus bus, ILogger<RabbitmqClient> logger)
    {
        _bus = bus;
        _logger = logger;
    }
    
    public async Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken) where TMessage : IMessage
    {
        _logger.LogInformation($"Sending a message: {message.GetType().Name}");
        await _bus.PubSub.PublishAsync(message, cancellationToken);
    }
}