using Messier.CQRS.Commands.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Messier.CQRS.Commands.CommandDispatcher;

internal sealed class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    internal CommandDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task DispatchAsync<TCommand>(TCommand command) where TCommand : class, ICommand
    {
        var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();
        await handler.HandleCommandAsync(command);
    }
}