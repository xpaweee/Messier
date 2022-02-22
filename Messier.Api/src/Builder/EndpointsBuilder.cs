using Messier.Api.Interfaces;
using Messier.Api.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Messier.Api.Builder;

internal sealed class EndpointsBuilder : IEndpointBuilder
{
    private readonly IEndpointRouteBuilder _routeBuilder;
    private readonly WebApiDefinitionList _webApiDefinitionList;

    public EndpointsBuilder(IEndpointRouteBuilder routeBuilder, WebApiDefinitionList webApiDefinitionList)
    {
        _routeBuilder = routeBuilder;
        _webApiDefinitionList = webApiDefinitionList;
    }

    public IEndpointBuilder AddGet<TQuery, TResult>(string path, Func<TQuery, HttpContext, Task> handler)
    {
        // var queryDispatcher = _httpContextAccessor.HttpContext?.RequestServices.GetRequiredService<IQueryDispatcher>();
        // queryDispatcher.DispatchAsync<TResult, TQuery>();
        AddQueryRouting<TQuery,TResult>(path);
        _routeBuilder.MapGet(path, handler);
        
        return this;
    }
    
    public IEndpointBuilder AddPost<TCommand>(string path, Func<TCommand, HttpContext, Task> context)
    {
        _routeBuilder.MapPost(path,context);
        AddCommandRouting<TCommand>(path);
        
        return this;
    }

    private void AddQueryRouting<TQuery, TResult>(string path)
        => AddRouting(path, HttpMethod.Get, typeof(TQuery), typeof(TResult));

    private void AddCommandRouting<TCommand>(string path)
        => AddRouting(path, HttpMethod.Post, typeof(TCommand), null!);

    private void AddRouting(string path, HttpMethod httpMethod, Type requestType, Type resultType)
    {
        _webApiDefinitionList.Add(new WebApiDefinition()
        {
            Method = httpMethod,
            Path = path,
            RequestType = requestType,
            ResultType = resultType
        });
    }
}