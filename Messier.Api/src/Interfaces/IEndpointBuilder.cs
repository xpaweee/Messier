namespace Messier.Api.Interfaces;

public interface IEndpointBuilder
{
    public IEndpointBuilder AddGet(string path);
    public IEndpointBuilder AddPost(string path);

}