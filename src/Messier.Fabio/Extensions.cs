using Messier.Fabio.HttpHandler;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messier.Fabio;

public static class Extensions
{
    private static readonly string sectionName = "fabbio";
    
    public static IServiceCollection AddFabio(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var section = configuration.GetSection("fabio");
        var options = section.BindOptions<FabioOptions>();
        serviceCollection.Configure<FabioOptions>(section);

        if (!options.Enabled)
        {
            return serviceCollection;
        }

        if (string.IsNullOrEmpty(options.Url))
        {
            throw new ArgumentNullException("Fabio url is empty.");
        }
   
        serviceCollection.AddTransient<FabioHttpHandler>();

        return serviceCollection;

    }

    public static IHttpClientBuilder AddFabioHandler(this IHttpClientBuilder builder)
    {
        builder.AddHttpMessageHandler<FabioHttpHandler>();

        return builder;
    }
}