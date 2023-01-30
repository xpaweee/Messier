namespace Messier.Postgres.OutboxPattern.Base;

public class OutboxOptions
{
        public bool Enabled { get; set; }
        public TimeSpan SenderInterval { get; set; }
        public TimeSpan CleanupInterval { get; set; }
}