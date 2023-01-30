using System.Reflection;
using System.Text.Json;
using Humanizer;
using Messier.Api;
using Messier.Interfaces;
using Messier.Messaging;
using Messier.Messaging.Interfaces;
using Messier.Postgres.OutboxPattern.Base;
using Messier.Postgres.OutboxPattern.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

namespace Messier.Postgres.OutboxPattern;

internal sealed class OutboxPostgres<TType> : IOutboxPostgres where TType : DbContext
{
    private readonly TType _dbContext;
    private readonly IClock _clock;
    private readonly IMessageContextAccessor _messageContextAccessor;
    private readonly DbSet<OutboxMessage> _outboxMessageEntity;
    private readonly ILogger<OutboxPostgres<TType>> _logger;
    private readonly IList<OutboxMessage> _localMessages;
    private readonly IMessageClient _messageClient;
    private readonly IJsonSerializer _jsonSerializer;
    private readonly MethodInfo _sendMethod;

    public OutboxPostgres(TType dbContext, IClock clock, IMessageContextAccessor messageContextAccessor, ILogger<OutboxPostgres<TType>> logger, IMessageClient messageClient, IJsonSerializer jsonSerializer)
    {
        _dbContext = dbContext;
        _clock = clock;
        _messageContextAccessor = messageContextAccessor;
        _logger = logger;
        _messageClient = messageClient;
        _jsonSerializer = jsonSerializer;
        _outboxMessageEntity = _dbContext.Set<OutboxMessage>();
        _localMessages = new List<OutboxMessage>();
        _sendMethod = messageClient.GetType().GetMethod(nameof(IMessageClient.SendAsync)) ??
                      throw new InvalidOperationException("Message broker send method was not defined.");
    }

    public async Task SaveMessageAsync<TMessage>(MessageWrapper<TMessage> message, CancellationToken cancellationToken)
        where TMessage : IMessage
    {
        var outBoxMessage = new OutboxMessage()
        {
            Id = message.Context.MessageId,
            Name = message.GetType().Name.Underscore(),
            Context = _jsonSerializer.Serialize(message.Context),
            Data = _jsonSerializer.Serialize(message.Message),
            Type = message.GetType().AssemblyQualifiedName ?? string.Empty,
            CreateDate = _clock.Current()
        };

        await _outboxMessageEntity.AddAsync(outBoxMessage);
        await _dbContext.SaveChangesAsync();
        _localMessages.Add(outBoxMessage);
        _logger.LogInformation($"Saving {outBoxMessage.Name} to outbox entity.");
    }

    public async Task ClearMessagesAsync(CancellationToken cancellationToken)
    {
        var messages = await _outboxMessageEntity
            .Where(x => x.SentDate != null)
            .OrderBy(x => x.CreateDate)
            .ToListAsync();
        _outboxMessageEntity.RemoveRange(messages);
        _logger.LogInformation("Removed all not sent messages from outbox entity.");
    }


    public async Task SendMessagesAsync(CancellationToken cancellationToken)
    {
        var messages = await _outboxMessageEntity.Where(x => x.SentDate == null).ToListAsync(cancellationToken);
        _logger.LogInformation($"Found {messages.Count} in outbox entity");

        foreach (var message in messages)
        {
            await PublishMessageAsync(message, cancellationToken);
            message.SentDate = _clock.Current();
            _dbContext.Entry(message).Property(x => x.SentDate).IsModified = true;
        }
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task PublishMessageAsync(OutboxMessage message, CancellationToken cancellationToken)
    {
        var context = _jsonSerializer.Deserialize<Context>(message.Context) ?? new Context();
        var messageContext = new MessageContext(message.Id, context);
        var messageType = Type.GetType(message.Type);
        var messageData = _jsonSerializer.Deserialize<IMessage>(message.Data);
        var messageWrapper = Activator.CreateInstance(messageType, messageData, messageContext);

        _logger.LogInformation("Sending message from outbox");
        var sendMessageTask = _sendMethod.Invoke(_messageClient, new[] { messageWrapper, cancellationToken });

        await (Task) sendMessageTask!;
    }
}