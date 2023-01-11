using System.Diagnostics;
using Humanizer;
using Messier.Api;
using Messier.Interfaces;
using Messier.Messaging.RabbitMQ.Interfaces;

namespace Messier.Observability.Jaeger.Decorators;

public class MessageHandlerTracingDecorator : IMessageHandler
{
    public const string ActivitySourceName = "message_handler";
    private static readonly ActivitySource ActivitySource = new(ActivitySourceName);
    private readonly IMessageHandler _messageHandler;
    private readonly IContextProvider _contextProvider;

    public MessageHandlerTracingDecorator(IMessageHandler messageHandler, IContextProvider contextProvider)
    {
        _messageHandler = messageHandler;
        _contextProvider = contextProvider;
    }

    public async Task HandleAsync<TMessage>(Func<IServiceProvider, TMessage, CancellationToken, Task> handler, TMessage message, CancellationToken cancellationToken = default) where TMessage : IMessage
    {
        var context = _contextProvider.Current();
        var name = message.GetType().Name.Underscore();
        using var activity = ActivitySource.StartActivity("subscriber", ActivityKind.Consumer, context.ActivityId);
        activity?.SetTag("message", name);
        activity?.SetTag("correlation_id", context.CorrelationId);
        try
        {
            await _messageHandler.HandleAsync(handler, message, cancellationToken);
        }
        catch (Exception exception)
        {
            activity?.SetStatus(ActivityStatusCode.Error, exception.ToString());
            throw;
        }
    }
}