namespace Messier.CQRS.Commands.Interfaces;

public interface ICommandDispatcher
{
    public Task DispatchAsync<TCommand>(TCommand command) where TCommand : class, ICommand;
}