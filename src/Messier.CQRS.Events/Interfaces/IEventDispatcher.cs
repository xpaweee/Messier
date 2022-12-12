namespace Messier.CQRS.Events.Interfaces;

public interface IEventDispatcher
{
    Task DispatchAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : class, IEvent;

}