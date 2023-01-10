using System.Diagnostics;
using Humanizer;
using Messier.Interfaces;
using Messier.Messaging.Interfaces;

namespace Messier.Observability.Jaeger.Decorators;

public class MessageBrokerTracingDecorator : IMessageClient
{
    public const string ActivitySourceName = "message_broker";
    private readonly IMessageClient _messageClient;
    private static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    public MessageBrokerTracingDecorator(IMessageClient messageClient)
    {
        _messageClient = messageClient;
    }

    public async Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken) where TMessage : IMessage
    {
        var name = message.GetType().Name.Underscore();
        
        using var activity = ActivitySource.StartActivity("publisher", ActivityKind.Producer);
        activity?.SetTag("message", name);
        await _messageClient.SendAsync(message, cancellationToken);
    }
}