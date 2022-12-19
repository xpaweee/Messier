using Messier.Interfaces;

namespace Messier.CQRS.Commands.Interfaces;

public interface ICommandDispatcher
{
    public Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : class, ICommand;
}