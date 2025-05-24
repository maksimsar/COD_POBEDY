namespace AuthService.Models;

public class OutboxMessage
{
    public long   Id            { get; set; }
    public string Type          { get; set; }
    public string Payload       { get; set; }
    public DateTimeOffset OccurredOnUtc { get; set; }
    public bool   Processed     { get; set; }
}