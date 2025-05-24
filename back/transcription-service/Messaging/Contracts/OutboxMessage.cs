namespace TranscriptionService.Messaging.Contracts;

public class OutboxMessage
{
    public long Id { get; set; }
    
    public string Type { get; set; } = null!;
    
    public string Payload { get; set; } = null!;
    
    public DateTimeOffset OccurredOnUtc { get; set; }
    
    public bool Processed { get; set; }
}