using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Messier.Postgres;

public class MigrationInitializer<TType> : IHostedService where TType : DbContext
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MigrationInitializer<TType>> _logger;

    public MigrationInitializer(IServiceProvider serviceProvider, ILogger<MigrationInitializer<TType>> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting to initialize database migration.");
        await using var scope = _serviceProvider.CreateAsyncScope();
        var dbContext = (DbContext) scope.ServiceProvider.GetRequiredService<TType>();
        await dbContext.Database.MigrateAsync(cancellationToken);
        _logger.LogInformation("Database migration initialized.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}