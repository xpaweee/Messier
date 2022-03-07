namespace Messier.CQRS.Commands.Interfaces;

public interface ICommandHandler<in TCommand> where TCommand : class, ICommand
{
    Task HandleCommandAsync(TCommand command);
}