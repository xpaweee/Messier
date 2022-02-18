using Messier.CQRS.Events.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Messier.CQRS.Events.EventDispatcher;

internal sealed class EventDispatcher : IEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public EventDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task DispatchAsync<TEvent>(TEvent @event) where TEvent : class, IEvent
    {
        var handler = _serviceProvider.GetRequiredService<IEventHandler<TEvent>>();
        await handler.HandleAsync(@event);
    }
}