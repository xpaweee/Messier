using System.Diagnostics;
using Humanizer;
using Messier.Interfaces;
using Messier.Messaging.RabbitMQ.Interfaces;

namespace Messier.Observability.Jaeger.Decorators;

public class MessageHandlerTracingDecorator : IMessageHandler
{
    public const string ActivitySourceName = "message_handler";
    private static readonly ActivitySource ActivitySource = new(ActivitySourceName);
    private readonly IMessageHandler _messageHandler;

    public MessageHandlerTracingDecorator(IMessageHandler messageHandler)
    {
        _messageHandler = messageHandler;
    }

    public async Task HandleAsync<TMessage>(Func<IServiceProvider, TMessage, CancellationToken, Task> handler, TMessage message, CancellationToken cancellationToken = default) where TMessage : IMessage
    {
        var name = message.GetType().Name.Underscore();
        using var activity = ActivitySource.StartActivity("subscriber", ActivityKind.Consumer);
        activity?.SetTag("message", name);
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