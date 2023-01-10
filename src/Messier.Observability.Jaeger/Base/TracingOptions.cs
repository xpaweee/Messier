namespace Messier.Observability.Jaeger.Base;

public class TracingOptions
{
    public bool Enabled { get; set; }
    public JaegerOptions Jaeger { get; set; }
}

public class JaegerOptions
{
    public string AgentHost { get; set; }
    public int AgentPort { get; set; } 
}