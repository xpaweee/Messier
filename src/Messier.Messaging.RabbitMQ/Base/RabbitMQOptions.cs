namespace Messier.Messaging.RabbitMQ.Base;

public sealed class RabbitMQOptions
{
    public bool Enabled { get; set; }
    public string ConnectionString { get; set; }
}