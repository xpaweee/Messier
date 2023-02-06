using System.Diagnostics;
using Humanizer;
using Messier.Api;
using Messier.Interfaces;
using Messier.Messaging.Interfaces;

namespace Messier.Observability.Jaeger.Decorators;

public class MessageBrokerTracingDecorator : IMessageBroker
{
    public const string ActivitySourceName = "message_broker";
    private readonly IMessageBroker _messageBroker;
    private static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    private readonly IContextProvider _contextProvider;

    public MessageBrokerTracingDecorator(IMessageBroker messageBroker, IContextProvider contextProvider)
    {
        _messageBroker = messageBroker;
        _contextProvider = contextProvider;
    }

    public async Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken) where TMessage : IMessage
    {
        var context = _contextProvider.Current();
        var name = message.GetType().Name.Underscore();
        using var activity = ActivitySource.StartActivity("publisher", ActivityKind.Producer, context.ActivityId);
        activity?.SetTag("message", name);
        activity?.SetTag("correlation_id", context.CorrelationId);
        await _messageBroker.SendAsync(message, cancellationToken);
    }
}