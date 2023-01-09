namespace Messier.Logging.Serilog.Base;

public class SerilogOptions
{
    public SeqOptions Seq { get; set; }
}

public class SeqOptions
{
    public bool Enabled { get; set; }
    public string Url { get; set; }
    
    public string Apikey { get; set; }
}