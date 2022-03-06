using Messier.MessageBroker.RawRabbit.Publisher.Interfaces;
using RawRabbit;

namespace Messier.MessageBroker.RawRabbit.Publisher;

public class BusPublisher : IBusPublisher
{
    private readonly IBusClient _busClient;

    public BusPublisher(IBusClient busClient)
    {
        _busClient = busClient;
    }

    public Task PublishAsync<TMessage>(TMessage message) where TMessage : class
    {
        return _busClient.PublishAsync(message);
    }
   
}
    
