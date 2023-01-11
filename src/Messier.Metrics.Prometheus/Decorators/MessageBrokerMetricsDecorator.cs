using System.Diagnostics.Metrics;
using Humanizer;
using Messier.Interfaces;
using Messier.Messaging.Interfaces;
using Messier.Metrics.Prometheus.Attributes;

namespace Messier.Metrics.Prometheus.Decorators;

[Meter(MetricsKey)]
public class MessageBrokerMetricsDecorator : IMessageClient
{
    private const string MetricsKey = "message_broker";
    
    private readonly IMessageClient _messageClient;
    private static readonly Meter Meter = new(MetricsKey);
    private static readonly Counter<long> PublishedMessagesCounter = Meter.CreateCounter<long>("published_messages");
    public MessageBrokerMetricsDecorator(IMessageClient messageClient)
    {
        _messageClient = messageClient;
    }

    public async Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken) where TMessage : IMessage
    {
        var messageName = message.GetType().Name.Underscore();
        await _messageClient.SendAsync(message, cancellationToken);
        
        PublishedMessagesCounter.Add(1, new KeyValuePair<string, object?>("message", messageName));
    }
}