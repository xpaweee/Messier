namespace Messier.CQRS.Events.Interfaces;

public interface IEventDispatcher
{
    Task DispatchAsync<TEvent>(TEvent @event) where TEvent : class, IEvent;
}