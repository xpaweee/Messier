using Messier.Postgres.OutboxPattern.Base;
using Messier.Postgres.OutboxPattern.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Messier.Postgres.OutboxPattern;

public class OutboxCleaner : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxCleaner> _logger;
    private readonly IOptions<OutboxOptions> _options;
    private int _isProcessing;

    public OutboxCleaner(IServiceProvider serviceProvider, ILogger<OutboxCleaner> logger, IOptions<OutboxOptions> options)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Cleaning outbox...");

        while (!stoppingToken.IsCancellationRequested)
        {

            if (Interlocked.Exchange(ref _isProcessing, 1) == 1)
            {
                await Task.Delay(_options.Value.CleanupInterval, stoppingToken);
                continue;
            }
            
            using var scope = _serviceProvider.CreateAsyncScope();
            try
            {
                var outbox = scope.ServiceProvider.GetRequiredService<IOutboxPostgres>();

                await outbox.ClearMessagesAsync(stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError("There was an error while cleaning outbox entity");
            }
            finally
            {
                Interlocked.Exchange(ref _isProcessing, 0);
            }

            await Task.Delay(_options.Value.CleanupInterval, stoppingToken);
        }
    }
}