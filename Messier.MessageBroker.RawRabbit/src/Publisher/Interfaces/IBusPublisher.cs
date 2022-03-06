namespace Messier.MessageBroker.RawRabbit.Publisher.Interfaces;

public interface IBusPublisher
{
    Task PublishAsync<TMessage>(TMessage message) where TMessage : class;
}