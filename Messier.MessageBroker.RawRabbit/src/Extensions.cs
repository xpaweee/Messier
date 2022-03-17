using Messier.CQRS.Commands.Interfaces;
using Messier.CQRS.Events.Interfaces;
using Messier.Interfaces;
using Messier.MessageBroker.RawRabbit.Base;
using Messier.MessageBroker.RawRabbit.Subscriber.Interaces;
using Microsoft.Extensions.DependencyInjection;
using RawRabbit;
using RawRabbit.Common;
using RawRabbit.Configuration;
using RawRabbit.Enrichers.MessageContext;
using RawRabbit.Instantiation;

namespace Messier.MessageBroker.RawRabbit;

public static class Extensions
{
    public const string _sectionName = "rabbitmq";

    private static IMessierBuilder AddRawRabbit(this IMessierBuilder builder)
    {
        var options = builder.GetRabbitOptions();
        builder.ServiceCollection.AddSingleton(options);

        ConfigureRawRabbit(builder);

        return builder;
    }

    private static void ConfigureRawRabbit(this IMessierBuilder builder)
    {
        builder.ServiceCollection.AddSingleton<IInstanceFactory>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<RabbitOptions>();
            var configuration = serviceProvider.GetRequiredService<RawRabbitConfiguration>();
            var namingConventions = new CustomNamingConventions(options.Namespace);

            return RawRabbitFactory.CreateInstanceFactory(new RawRabbitOptions
            {
                DependencyInjection = ioc =>
                {
                    ioc.AddSingleton(options);
                    ioc.AddSingleton(configuration);
                    ioc.AddSingleton(serviceProvider);
                    ioc.AddSingleton<INamingConventions>(namingConventions);
                },
                Plugins = p =>
                {
                    p.UseAttributeRouting()
                        .UseRetryLater()
                        .UseMessageContext<TContext>()
                        .UseContextForwarding();

                    if (options.MessageProcessor?.Enabled == true)
                    {
                        p.ProcessUniqueMessages();
                    }
                }
            });
        });
    }
    public static IBusSubscriber SubscribeCommand<TCommand>(this IBusSubscriber subscriber)
        where TCommand : class, ICommand 
        => subscriber.Subscribe<TCommand>(async (provider, message) =>
        {
            await using var scope = provider.CreateAsyncScope();
            var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<TCommand>>();
            await handler.HandleCommandAsync(message);

        });
    public static IBusSubscriber SubscribeEvent<TEvent>(this IBusSubscriber subscriber,
        Func<IServiceProvider, TEvent, Task> context) where TEvent : class, IEvent
        => subscriber.Subscribe<TEvent>( async (provider, @event) =>
        {
            await using var scope = provider.CreateAsyncScope();
            var handler = scope.ServiceProvider.GetRequiredService<IEventHandler<TEvent>>();
            await handler.HandleEventAsync(@event);
        });
    
    public static RabbitOptions GetRabbitOptions(this IMessierBuilder builder)
        => builder.GetOptions<RabbitOptions>(_sectionName);
}