using Messier.Messaging.Interfaces;
using Messier.Postgres.Base;
using Messier.Postgres.OutboxPattern;
using Messier.Postgres.OutboxPattern.BackgroundProcesses;
using Messier.Postgres.OutboxPattern.Base;
using Messier.Postgres.OutboxPattern.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messier.Postgres;

public static class Extensions
{
    private static string _postgresSectionName = "postgres";
    private static string _outboxSectionName = "outbox";
    
    public static IServiceCollection AddPostgres<TType>(this IServiceCollection serviceCollection, IConfiguration configuration) where TType : DbContext
    {
        var section = configuration.GetSection(_postgresSectionName);
        var options = section.BindOptions<PostgresOptions>();
        serviceCollection.Configure<PostgresOptions>(section);
        if (options.ConnectionString == string.Empty)
        {
            return serviceCollection;
        }
        serviceCollection.AddDbContext<TType>(x => x.UseNpgsql(options.ConnectionString));
        serviceCollection.AddHostedService<MigrationInitializer<TType>>();
        //https://github.com/npgsql/efcore.pg/issues/2000
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        
        return serviceCollection;
    }

    public static IServiceCollection AddOutbox<TType>(this IServiceCollection serviceCollection, IConfiguration configuration) where TType : DbContext
    {
        var section = configuration.GetSection(_outboxSectionName);
        var outboxOptions = section.BindOptions<OutboxOptions>();
        serviceCollection.Configure<OutboxOptions>(section);
        serviceCollection.AddScoped<IOutboxPostgres, OutboxPostgres<TType>>();
        
        if (!outboxOptions.Enabled)
        {
            return serviceCollection;
        }
        
        serviceCollection.AddTransient<IMessageBroker, OutboxMessageBroker>();
        serviceCollection.AddHostedService<OutboxSender>();
        // serviceCollection.AddHostedService<OutboxCleaner>();

        return serviceCollection;
    }
}