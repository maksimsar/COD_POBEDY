using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MetadataService.Data;

[Table("outbox_messages")]
public sealed class OutboxMessage
{
    [Key] public long   Id          { get; init; }
    public string Type  { get; init; } = default!;
    public string Payload { get; init; } = default!;
    public DateTime OccurredAt { get; init; }
    public DateTime? SentAt    { get; set; }
}