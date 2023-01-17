using Messier.Postgres.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messier.Postgres;

public static class Extensions
{
    private static string _sectionName = "postgres";
    
    public static IServiceCollection AddPostgres<TType>(this IServiceCollection serviceCollection, IConfiguration configuration) where TType : DbContext
    {
        var section = configuration.GetSection(_sectionName);
        var options = section.BindOptions<PostgresOptions>();
        serviceCollection.Configure<PostgresOptions>(section);
        if (options is null)
        {
            return serviceCollection;
        }
        serviceCollection.AddDbContext<TType>(x => x.UseNpgsql(options.ConnectionString));
        serviceCollection.AddHostedService<MigrationInitializer<TType>>();
        //https://github.com/npgsql/efcore.pg/issues/2000
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        
        return serviceCollection;
    }
}