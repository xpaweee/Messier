namespace Messier.Api.Models;

public class WebApiDefinition
{
    public HttpMethod Method { get; set; }
    public string Path { get; set; }
    public Type ResultType { get; set; }
    public Type RequestType { get; set; }
}