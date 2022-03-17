using Messier.MessageBroker.RawRabbit.Subscriber.Interaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RawRabbit;

namespace Messier.MessageBroker.RawRabbit.Subscriber;

public class BusSubscriber : IBusSubscriber
{
    private readonly IBusClient _busClient;
    private readonly IServiceProvider _serviceProvider;

    public BusSubscriber(IApplicationBuilder app,IBusClient busClient)
    {
        _busClient = busClient;
        _serviceProvider =  app.ApplicationServices.GetRequiredService<IServiceProvider>();
    }
    public IBusSubscriber Subscribe<T>(Func<IServiceProvider, T, Task> context)
    {
        _busClient.SubscribeAsync<T>(async (method) =>
        {
            await context.Invoke(_serviceProvider, method);
            
        }).GetAwaiter().GetResult();

        return this;
    }
}