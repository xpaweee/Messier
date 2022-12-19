using Messier.Interfaces;

namespace Messier.CQRS.Commands.Interfaces;

public interface ICommandHandler<in TCommand> where TCommand : class, ICommand
{
    Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}
