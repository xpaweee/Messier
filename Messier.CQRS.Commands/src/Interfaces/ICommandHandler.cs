namespace Messier.CQRS.Commands.Interfaces;

internal interface ICommandHandler<in TCommand> where TCommand : class, ICommand
{
    Task HandleAsync(TCommand command);
}