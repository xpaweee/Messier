using System.Net;

namespace Messier.Api.Exception.Model;

public class ExceptionResponse
{
    public HttpStatusCode HttpStatusCode { get; set; }
    public string Reponse { get; set; }
}