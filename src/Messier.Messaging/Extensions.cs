using Messier.Messaging.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Messier.Messaging;

public static class Extensions
{
    public static IServiceCollection AddMessaging(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IMessageBroker, MessageBroker>();

        return serviceCollection;
    }
}