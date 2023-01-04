namespace Messier.Messaging.RabbitMQ.Interfaces;

public interface IMessageTypeRegistry
{
    void Register<T>();
    Type? Resolve(string type);
}