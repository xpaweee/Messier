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

    async Task ICommandDispatcher.DispatchAsync<TCommand>(TCommand command) where TCommand : class
    {
        var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();
        await handler.HandleAsync(command);
    }
}