
namespace Messier.MessageBroker.RawRabbit.Subscriber.Interaces;

public interface IBusSubscriber
{
    IBusSubscriber Subscribe<T>(Func<IServiceProvider, T, Task> context);
}