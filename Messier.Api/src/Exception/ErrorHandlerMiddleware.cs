using Messier.Api.Exception.Interfaces;
using Messier.Api.Exception.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Messier.Api.Exception;

public class ErrorHandlerMiddleware : IMiddleware
{
    private ILogger<ErrorHandlerMiddleware> _logger;
    private IExceptionToMessageMapper _exceptionToMessageMapper;

    public ErrorHandlerMiddleware(ILogger<ErrorHandlerMiddleware> logger, IExceptionToMessageMapper exceptionToMessageMapper)
    {
        _logger = logger;
        _exceptionToMessageMapper = exceptionToMessageMapper;
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
        
        //TODO: Add converting to json
        //context.Response.Body = exceptionResponse.Reponse;

    }
}