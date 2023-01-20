using Consul;
using Consul.Filtering;
using Messier.Consul.Base;
using Microsoft.Extensions.Options;

namespace Messier.Consul.HttpHandler;

public class ConsulHttpHandler : DelegatingHandler
{
    private readonly IConsulClient _consulClient;
    private readonly IOptions<ConsulOptions> _options;
    private readonly StringFieldSelector _selector = new("Service");
    private const string _hostDockerInternal = "host.docker.internal";

    public ConsulHttpHandler(IConsulClient consulClient, IOptions<ConsulOptions> options)
    {
        _consulClient = consulClient;
        _options = options;
    }
    
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!_options.Value.Enabled)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        if (request.RequestUri is null)
        {
            return await base.SendAsync(request, cancellationToken);
        }
        
        var serviceName = request.RequestUri.Host;
        if (string.IsNullOrWhiteSpace(serviceName))
        {
            return await base.SendAsync(request, cancellationToken);
        }

        var filter = _selector == serviceName;
        var services = await _consulClient.Agent.Services(filter, cancellationToken);
        if (!services.Response.Any())
        {
            throw new KeyNotFoundException();
        }

        var service = services.Response.First().Value;
        if (service.Address.Contains(_hostDockerInternal))
        {
            service.Address = service.Address.Replace(_hostDockerInternal, "localhost");
        }
        
        var uriBuilder = new UriBuilder(request.RequestUri)
        {
            Host = service.Address,
            Port = service.Port
        };
        
        request.RequestUri = uriBuilder.Uri;

        return await base.SendAsync(request, cancellationToken);
    }
}