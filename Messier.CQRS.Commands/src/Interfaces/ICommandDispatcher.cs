namespace Messier.CQRS.Commands.Interfaces;

public interface ICommandDispatcher
{
    internal Task DispatchAsync<TCommand>(TCommand command) where TCommand : class, ICommand;
}