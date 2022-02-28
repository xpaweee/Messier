using Messier.Api.Interfaces;
using Messier.CQRS.Commands.Interfaces;
using Messier.CQRS.Queries.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Messier.Api;

public class EndpointsDispatcher : IEndpointDispatcher
{
    private readonly IEndpointBuilder _endpointBuilder;

    public EndpointsDispatcher(IEndpointBuilder endpointBuilder)
    {
        _endpointBuilder = endpointBuilder;
    }
    
    
    public IEndpointDispatcher AddGet<TQuery,TResult>(string path) where TQuery: class, IQuery where TResult : class
    {
        _endpointBuilder.AddGet<TQuery, TResult>(path, async (query, context) =>
        {
            var dispatcher = context.RequestServices.GetRequiredService<IQueryDispatcher>();
            var result = await dispatcher.DispatchAsync<TQuery, TResult>(query);

            if (result is null)
            {
                context.Response.StatusCode = 404;
                return;
            }

            await context.Response.WriteAsJsonAsync(result);

        });
        
        return this;
    }


    public IEndpointDispatcher AddPost<TCommand>(string path) where TCommand : class, ICommand
    {
        _endpointBuilder.AddPost<TCommand>(path, async (command, ctx) =>
        {
            var commandDispatcher = ctx.RequestServices.GetService<ICommandDispatcher>();

            if (commandDispatcher is null)
            {
                ctx.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            await commandDispatcher.DispatchAsync(command);

        });

        return this;
    }
}