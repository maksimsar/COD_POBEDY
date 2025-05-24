using System;

namespace ProcessingService.DTOs
{
    public class DenoiseResult
    {
        public Guid RecordId { get; set; }
        public string ProcessedUrl { get; set; } = null!;
    }
}
