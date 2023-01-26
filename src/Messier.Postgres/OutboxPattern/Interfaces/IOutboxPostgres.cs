using Messier.Interfaces;
using Messier.Messaging;

namespace Messier.Postgres.OutboxPattern.Interfaces;

public interface IOutboxPostgres
{
    Task SaveMessageAsync<TMessage>(MessageWrapper<TMessage> message, CancellationToken cancellationToken)
        where TMessage : IMessage;

    Task ClearMessagesAsync(CancellationToken cancellationToken);

    Task SendMessagesAsync(CancellationToken cancellationToken);
}