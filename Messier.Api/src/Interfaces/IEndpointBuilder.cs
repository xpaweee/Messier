namespace Messier.Api.Interfaces;

public interface IEndpointBuilder
{
    IEndpointBuilder AddGet<TResult, TQuery>(string path);
    IEndpointBuilder AddPost<TQuery>(string path);

}