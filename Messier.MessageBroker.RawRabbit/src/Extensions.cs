using Messier.CQRS.Commands.Interfaces;
using Messier.CQRS.Events.Interfaces;
using Messier.Interfaces;
using Messier.MessageBroker.RawRabbit.Base;
using Messier.MessageBroker.RawRabbit.Subscriber.Interaces;
using Microsoft.Extensions.DependencyInjection;
using RawRabbit.Instantiation;

namespace Messier.MessageBroker.RawRabbit;

public static class Extensions
{
    public const string _sectionName = "rabbitmq";

    public static IMessierBuilder AddRawRabbit(this IMessierBuilder builder)
    {
        var options = builder.GetRabbitOptions();
        builder.ServiceCollection.AddSingleton(options);

        RawRabbitFactory.CreateInstanceFactory(new RawRabbitOptions()
        {
            DependencyInjection = x =>
            {

            }
        });

        return builder;
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