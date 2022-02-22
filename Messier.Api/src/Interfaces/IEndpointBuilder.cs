using Microsoft.AspNetCore.Http;

namespace Messier.Api.Interfaces;

public interface IEndpointBuilder
{
    IEndpointBuilder AddGet<TQuery, TResult>(string path, Func<TQuery, HttpContext, Task> context);
    IEndpointBuilder AddPost<TCommand>(string path, Func<TCommand, HttpContext, Task> context);

}