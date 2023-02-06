namespace Messier.Logging.Serilog.Base;

public class SerilogOptions
{
    public SeqOptions Seq { get; set; }
    
    public ConsoleOptions Console { get; set; }
    public IEnumerable<string>? NoTrackingUrls { get; set; }
}

public class ConsoleOptions
{
    public bool Enabled { get; set; }
}
public class SeqOptions
{
    public bool Enabled { get; set; }
    public string Url { get; set; }
    public string Apikey { get; set; }
}

