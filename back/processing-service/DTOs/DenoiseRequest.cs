using System;

namespace ProcessingService.DTOs
{
    public class DenoiseRequest
    {
        public Guid RecordId { get; set; }
        public string OriginalUrl { get; set; } = null!;
    }
}
