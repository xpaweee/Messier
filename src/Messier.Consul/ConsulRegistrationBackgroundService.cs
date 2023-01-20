using System.Net;
using Consul;
using Messier.Consul.Base;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Messier.Consul;

public class ConsulRegistrationBackgroundService : IHostedService
{
    private readonly IConsulClient _consulClient;
    private readonly IOptions<ConsulOptions> _options;
    private readonly ILogger<ConsulRegistrationBackgroundService> _logger;
    private readonly string _serviceName;
    private readonly string _serviceId;

    public ConsulRegistrationBackgroundService(IConsulClient consulClient, ILogger<ConsulRegistrationBackgroundService> logger, IOptions<ConsulOptions> options)
    {
        _consulClient = consulClient;
        _logger = logger;
        _options = options;
        _serviceName = _options.Value.Service.Name;
        _serviceId = $"{_serviceName}_${Guid.NewGuid()}";
    }


    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Registering a service: '{_serviceId}' in Consul...");
        var serviceUrl = new Uri(_options.Value.Service.Url);
        var result = await _consulClient.Agent.ServiceRegister(new AgentServiceRegistration
        {
            ID = _serviceId,
            Name = _serviceName,
            Address = serviceUrl.Host,
            Port = serviceUrl.Port,
            Check = new AgentServiceCheck
            {
                HTTP = $"{serviceUrl}{_options.Value.HealthCheck.Endpoint}",
                Interval = _options.Value.HealthCheck.Interval,
                DeregisterCriticalServiceAfter = _options.Value.HealthCheck.DeregisterInterval
            }
        }, cancellationToken);
        if (result.StatusCode == HttpStatusCode.OK)
        {
            _logger.LogInformation($"Registered a service: '{_serviceId}' in Consul.");
            return;
        }
        
        _logger.LogError($"There was an error: {result.StatusCode} when registering a service: '{_serviceId}' in Consul.");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Deregistering a service: '{_serviceName}' in Consul...");
        var result = await _consulClient.Agent.ServiceDeregister(_serviceId, cancellationToken);
        if (result.StatusCode == HttpStatusCode.OK)
        {
            _logger.LogInformation($"Deregistered a service: '{_serviceId}' in Consul.");
            return;
        }
        
        _logger.LogError($"There was an error: {result.StatusCode} when deregistering a service: '{_serviceId}' in Consul.");
    }
}