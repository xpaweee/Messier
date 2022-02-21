using Messier.Api.Interfaces;
using Messier.Api.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Messier.Api;

internal sealed class EndpointsBuilder : IEndpointBuilder
{
    private readonly IEndpointRouteBuilder _routeBuilder;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly WebApiDefinitionList _webApiDefinitionList;

    public EndpointsBuilder(IEndpointRouteBuilder routeBuilder, IHttpContextAccessor httpContextAccessor, WebApiDefinitionList webApiDefinitionList)
    {
        _routeBuilder = routeBuilder;
        _httpContextAccessor = httpContextAccessor;
        _webApiDefinitionList = webApiDefinitionList;
    }

    public IEndpointBuilder AddGet<TQuery, TResult>(string path)
    {
        // var queryDispatcher = _httpContextAccessor.HttpContext?.RequestServices.GetRequiredService<IQueryDispatcher>();
        // queryDispatcher.DispatchAsync<TResult, TQuery>();
        AddQueryRouting<TQuery,TResult>(path);
        _routeBuilder.MapGet(path,null!);
        
        return this;
    }
    
    public IEndpointBuilder AddPost<TCommand>(string path)
    {
        _routeBuilder.MapPost(path,null!);
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