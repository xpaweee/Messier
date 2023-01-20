namespace Messier.Consul.Base;

public sealed class ConsulOptions
{
    public bool Enabled { get; set; }
    public string Url { get; set; }
    public ConsulServiceOption Service { get; set; }
    public ConsulHealthCheckOptions HealthCheck { get; set; }
}

public sealed class ConsulServiceOption
{
    public string Url { get; set; }
    public string Name { get; set; }
}
public sealed class ConsulHealthCheckOptions
{
    public string Endpoint { get; set; }
    public TimeSpan Interval { get; set; }
    public TimeSpan? DeregisterInterval { get; set; }
}