namespace Messier.Postgres.OutboxPattern.Base;

public class OutboxMessage
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; }
    public string Data { get; set; }
    public string Context { get; set; }
    public string Type { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? SentDate { get; set; }
}