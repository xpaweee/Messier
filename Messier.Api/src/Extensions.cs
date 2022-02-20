using Microsoft.AspNetCore.Builder;

namespace Messier.Api;

public static class Extensions
{
    public static IApplicationBuilder UseWebApiEndpoints(this IApplicationBuilder app, bool useAuhtorization = true)
    {
        app.UseRouting();  
        
        //TODO: Add use authorization
        // if(useAuhtorization)
        
        app.UseEndpoints(endpoints =>
        {
           
        });

        return app;
    }
}