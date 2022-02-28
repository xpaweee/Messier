using Messier.Api.Exception.Model;

namespace Messier.Api.Exception.Interfaces;

public interface IExceptionToMessageMapper
{
    public ExceptionResponse MapExceptionToMessage(System.Exception exception);
}