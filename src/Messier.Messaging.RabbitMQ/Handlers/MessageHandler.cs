using Messier.Interfaces;
using Messier.Messaging.RabbitMQ.Interfaces;
using Microsoft.Extensions.Logging;

namespace Messier.Messaging.RabbitMQ.Handlers;

public class MessageHandler : IMessageHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MessageHandler> _logger;

    public MessageHandler(IServiceProvider serviceProvider, ILogger<MessageHandler> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    
    public async Task HandleAsync<TMessage>(Func<IServiceProvider, TMessage, CancellationToken, Task> handler, TMessage message, CancellationToken cancellationToken = default) where TMessage : IMessage
    {
        try
        {
            await handler(_serviceProvider, message, cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, exception.Message);
            throw;
        }    
    }
}