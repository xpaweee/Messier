using Humanizer;
using Messier.Interfaces;
using Messier.Messaging.Interfaces;
using Microsoft.Extensions.Logging;

namespace Messier.Postgres.OutboxPattern;

public class OutboxMessageBroker : IMessageClient
{
    private readonly ILogger<OutboxMessageBroker> _logger;

    public OutboxMessageBroker(ILogger<OutboxMessageBroker> logger)
    {
        _logger = logger;
    }
    public async Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken) where TMessage : IMessage
    {
        
        _logger.LogInformation($"Saving message to outbox: {message.GetType().Name.Underscore()}");
    }
}