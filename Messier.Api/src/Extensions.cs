using Messier.Api.Builder;
using Messier.Api.Interfaces;
using Messier.Api.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Messier.Api;

public static class Extensions
{
    public static IApplicationBuilder UseWebApiEndpointDispatchers(this IApplicationBuilder app, Action<IEndpointBuilder> builder,bool useAuhtorization = true)
    {
        app.UseRouting();  
        
        var webApiDefinitionList = app.ApplicationServices.GetRequiredService<WebApiDefinitionList>();
        
        app.UseEndpoints(endpointRouteBuilder =>
        {
            builder(new EndpointsBuilder(endpointRouteBuilder, webApiDefinitionList));
        });

        return app;
    }
}