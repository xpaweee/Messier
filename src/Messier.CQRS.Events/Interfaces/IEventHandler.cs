namespace Messier.CQRS.Events.Interfaces;

public interface IEventHandler<in TEvent> where TEvent : class, IEvent
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken);
}