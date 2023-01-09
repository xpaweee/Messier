namespace Messier.Interfaces;

public interface ICommandDispatcher
{
    public Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : class, ICommand;
}