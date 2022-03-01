using Messier.Api.Exception.Interfaces;
using Messier.Api.Exception.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Open.Serialization.Json;

namespace Messier.Api.Exception;

public class ErrorHandlerMiddleware : IMiddleware
{
    private readonly ILogger<ErrorHandlerMiddleware> _logger;
    private readonly IExceptionToMessageMapper _exceptionToMessageMapper;
    private readonly IJsonSerializer _jsonSerializer;

    public ErrorHandlerMiddleware(ILogger<ErrorHandlerMiddleware> logger, IExceptionToMessageMapper exceptionToMessageMapper, IJsonSerializer jsonSerializer)
    {
        _logger = logger;
        _exceptionToMessageMapper = exceptionToMessageMapper;
        _jsonSerializer = jsonSerializer;
    }
    
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (System.Exception e)
        {
            _logger.LogError(e, e.Message);
            HandleErrorAsync(context, e);
        }
    }

    private void HandleErrorAsync(HttpContext context, System.Exception exception)
    {
        ExceptionResponse exceptionResponse = _exceptionToMessageMapper.MapExceptionToMessage(exception);
        context.Response.StatusCode = (int)exceptionResponse.HttpStatusCode;
        
        context.Response.ContentType = "application/json";
        _jsonSerializer.SerializeAsync(context.Response.Body, exceptionResponse.Reponse);
    }
}