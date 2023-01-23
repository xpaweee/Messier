using Microsoft.Extensions.Options;

namespace Messier.Fabio.HttpHandler;

public class FabioHttpHandler : DelegatingHandler
{
    private readonly IOptions<FabioOptions> _options;

    public FabioHttpHandler(IOptions<FabioOptions> options)
    {
        _options = options;
    }
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var url = new Uri(_options.Value.Url);
        
        if (_options.Value.Enabled || request.RequestUri is null)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        var serviceName = request.RequestUri.Host;
        var uriBuilder = new UriBuilder(request.RequestUri)
        {
            Host = url.Host,
            Port = url.Port,    
            Path = $"{serviceName}{request.RequestUri.PathAndQuery}"
        };
        
        request.RequestUri = uriBuilder.Uri;

        return await base.SendAsync(request, cancellationToken);
    }
}