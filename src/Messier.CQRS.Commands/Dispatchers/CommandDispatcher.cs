using Messier.CQRS.Commands.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using ICommand = Messier.CQRS.Commands.Interfaces.ICommand;

namespace Messier.CQRS.Commands.Dispatchers;

public class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public CommandDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    

    public async Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : class, ICommand
    {
        using var scope = _serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<TCommand>>();
        await handler.HandleAsync(command, cancellationToken);
    }
}