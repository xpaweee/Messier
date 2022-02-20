using Messier.Api.Interfaces;
using Messier.CQRS.Queries.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Messier.Api;

public class EndpointsBuilder : IEndpointBuilder
{
    private readonly IEndpointRouteBuilder _routeBuilder;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EndpointsBuilder(IEndpointRouteBuilder routeBuilder, IHttpContextAccessor httpContextAccessor)
    {
        _routeBuilder = routeBuilder;
        _httpContextAccessor = httpContextAccessor;
    }

    public IEndpointBuilder AddGet<TResult, TQuery>(string path)
    {
        // _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<

        var queryDispatcher = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IQueryDispatcher>();

        queryDispatcher.DispatchAsync<TResult, TQuery>();
        _routeBuilder.MapGet(path,null!);
        
        return this;
    }
    
    public IEndpointBuilder AddPost(string path)
    {
        _routeBuilder.MapPost(path,null!);
        
        return this;
    }
}