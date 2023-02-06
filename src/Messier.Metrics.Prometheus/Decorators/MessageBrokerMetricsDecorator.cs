using System.Diagnostics.Metrics;
using Humanizer;
using Messier.Interfaces;
using Messier.Messaging.Interfaces;
using Messier.Metrics.Prometheus.Attributes;

namespace Messier.Metrics.Prometheus.Decorators;

[Meter(MetricsKey)]
public class MessageBrokerMetricsDecorator : IMessageBroker
{
    private const string MetricsKey = "message_broker";
    
    private readonly IMessageBroker _messageBroker;
    private static readonly Meter Meter = new(MetricsKey);
    private static readonly Counter<long> PublishedMessagesCounter = Meter.CreateCounter<long>("published_messages");
    public MessageBrokerMetricsDecorator(IMessageBroker messageBroker)
    {
        _messageBroker = messageBroker;
    }

    public async Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken) where TMessage : IMessage
    {
        var messageName = message.GetType().Name.Underscore();
        await _messageBroker.SendAsync(message, cancellationToken);
        
        PublishedMessagesCounter.Add(1, new KeyValuePair<string, object?>("message", messageName));
    }
}