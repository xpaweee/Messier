using Messier.Postgres.OutboxPattern.Base;
using Messier.Postgres.OutboxPattern.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Messier.Postgres.OutboxPattern.BackgroundProcesses;

public sealed class OutboxSender : BackgroundService
{
    private readonly ILogger<OutboxSender> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<OutboxOptions> _options;
    private int _isProcessing;

    public OutboxSender(ILogger<OutboxSender> logger, IServiceProvider serviceProvider, IOptions<OutboxOptions> options)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _options = options;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Value.Enabled)
        {
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
           
            if (Interlocked.Exchange(ref _isProcessing, 1) == 1)
            {
                await Task.Delay(_options.Value.SenderInterval, stoppingToken);
                continue;
            }
            
            _logger.LogInformation("Processing outbox messages");

            await using var scope = _serviceProvider.CreateAsyncScope();
            try
            {
                var outbox = scope.ServiceProvider.GetRequiredService<IOutboxPostgres>();
                await outbox.SendMessagesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("There was an error while processing outbox message");
                _logger.LogError(ex.Message, ex);
            }
            finally
            {
                Interlocked.Exchange(ref _isProcessing, 0);
            }

            await Task.Delay(_options.Value.SenderInterval, stoppingToken);
        }
    }
}