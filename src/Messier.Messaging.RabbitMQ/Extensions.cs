using EasyNetQ;
using Messier.Messaging.Interfaces;
using Messier.Messaging.RabbitMQ.Base;
using Messier.Messaging.RabbitMQ.Handlers;
using Messier.Messaging.RabbitMQ.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messier.Messaging.RabbitMQ;

public static class Extensions
{
    public static IServiceCollection AddRabbitmq(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var section = configuration.GetSection("rabbitmq");
        var options = section.BindOptions<RabbitMQOptions>();

        if (!options.Enabled)
        {
            return serviceCollection;
        }

        var bus = RabbitHutch.CreateBus(options.ConnectionString);
        
        serviceCollection.AddSingleton(bus);
        serviceCollection.AddSingleton<IMessageClient, RabbitmqClient>();
        serviceCollection.AddSingleton<IMessageSubscriber, RabbitmqSubscriber>();
        serviceCollection.AddSingleton<IMessageHandler, MessageHandler>();

        return serviceCollection;
    }
}