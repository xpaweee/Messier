using Humanizer;
using Messier.Interfaces;
using Microsoft.Extensions.Logging;

namespace Messier.Logging.Serilog.Decorators;

public class LoggingCommandHandlerDecorator <T> : ICommandHandler<T> where T : class, ICommand
{
    private readonly ILogger<LoggingCommandHandlerDecorator<T>> _logger;
    private readonly ICommandHandler<T> _commandHandler;

    public LoggingCommandHandlerDecorator(ILogger<LoggingCommandHandlerDecorator<T>> logger, ICommandHandler<T> commandHandler)
    {
        _logger = logger;
        _commandHandler = commandHandler;
    }
    
    public async Task HandleAsync(T command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Handling command{command.GetType().Name.Underscore()}");
        await _commandHandler.HandleAsync(command, cancellationToken);
        _logger.LogInformation($"Handled command {command.GetType().Name.Underscore()}");
    }
}