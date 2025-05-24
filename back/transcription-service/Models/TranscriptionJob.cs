using MassTransit.Contracts.JobService;

namespace TranscriptionService.Models;

public class TranscriptionJob : BaseEntity
{
    public Guid AudioFileId { get; init; }
    public string ModelUsed { get; set; } = "whisper";
    public string Language {get; set;} = "ru";
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset FinishedAt { get; set; }
    public string Status { get; set; } = JobStatus.Queued;
    public string? ErrorMessage { get; set; }

    public static class JobStatus 
    {
        public const string Queued = "В очереди";
        public const string Running = "В обработке";
        public const string Done = "Выполнено";
        public const string Error    = "Ошибка";
    }
}