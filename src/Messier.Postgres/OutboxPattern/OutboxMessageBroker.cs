using Humanizer;
using Messier.Api;
using Messier.Interfaces;
using Messier.Messaging;
using Messier.Messaging.Interfaces;
using Messier.Postgres.OutboxPattern.Interfaces;
using Microsoft.Extensions.Logging;

namespace Messier.Postgres.OutboxPattern;

internal sealed class OutboxMessageBroker : IMessageBroker
{
    private readonly IOutboxPostgres _outbox;
    private readonly ILogger<OutboxMessageBroker> _logger;
    private readonly IContextProvider _contextProvider;
    

    public OutboxMessageBroker(ILogger<OutboxMessageBroker> logger, IOutboxPostgres outbox, IContextProvider contextProvider)
    {
        _logger = logger;
        _outbox = outbox;
        _contextProvider = contextProvider;
    }
    public async Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken) where TMessage : IMessage
    {

        var messageId = Guid.NewGuid().ToString("N");
        var messageWrapper =
            new MessageWrapper<TMessage>(message, new MessageContext(messageId, _contextProvider.Current()));
        _logger.LogInformation($"Saving message to outbox: {message.GetType().Name.Underscore()}");
        await _outbox.SaveMessageAsync(messageWrapper, cancellationToken);
    }
}