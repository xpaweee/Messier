using Messier.Interfaces;

namespace Messier.Messaging.Interfaces;

public interface IMessageSubscriber
{
    IMessageSubscriber Command<TCommand>() where TCommand : class, ICommand;

    IMessageSubscriber Message<TCommand>(Func<IServiceProvider, TCommand, CancellationToken, Task> handler)
        where TCommand : class, IMessage;
}