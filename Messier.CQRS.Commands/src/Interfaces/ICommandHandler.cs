namespace Messier.CQRS.Commands.Interfaces;

internal interface ICommandHandler<in TCommand> where TCommand : class, ICommand
{
    Task HandleCommandAsync(TCommand command);
}