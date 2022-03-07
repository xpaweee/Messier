using Messier.MessageBroker.RawRabbit.Subscriber.Interaces;
using RawRabbit;

namespace Messier.MessageBroker.RawRabbit.Subscriber;

public class BusSubscriber : IBusSubscriber
{
    private readonly IBusClient _busClient;

    public BusSubscriber(IBusClient busClient)
    {
        _busClient = busClient;
    }
    public IBusSubscriber Subscribe<T>(Func<IServiceProvider, T, Task> context)
    {
        _busClient.SubscribeAsync()
    }
}