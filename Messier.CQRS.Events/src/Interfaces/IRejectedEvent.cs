namespace Messier.CQRS.Events.Interfaces;

public interface IRejectedEvent : IEvent
{
    public string Code { get; }
    public string Reason { get; }
}