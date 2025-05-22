using Common.Events;
using MassTransit;
using TranscriptionService.Data;
using TranscriptionService.Models;
using TranscriptionService.Repositories;

namespace TranscriptionService.Messaging.Consumers;

public sealed class FileUploadedConsumer : BaseConsumer<FileUploadedV2>
{
    private readonly ITranscriptionJobRepository _repo;
    private readonly ITranscriptionService       _service;
    private readonly IBackgroundTaskQueue        _bgQueue;   // простой wrapper вокруг Channel/Task

    public FileUploadedConsumer(
        ILogger<FileUploadedConsumer> log,
        TranscriptionContext db,
        IPublishEndpoint bus,
        ITranscriptionJobRepository repo,
        ITranscriptionService service,
        IBackgroundTaskQueue bgQueue) 
        : base(log, db, bus)
    {
        _repo     = repo;
        _service  = service;
        _bgQueue  = bgQueue;
    }

    protected override async Task HandleCoreAsync(FileUploadedV2 evt, CancellationToken ct)
    {
        var job = new TranscriptionJob
        {
            AudioFileId = evt.AudioFileId,
            Language    = "ru",
            ModelUsed   = "whisper"
        };

        _repo.Add(job);
        
        await _db.SaveChangesAsync(ct);

        _bgQueue.Enqueue(async token => 
            await _service.ProcessAsync(job.Id, evt.StorageKey, token));
    }
}