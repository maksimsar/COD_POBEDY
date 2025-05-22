using Common.Commands;
using Common.Events;
using MassTransit;
using MetadataService.Adapters;
using MetadataService.Infrastructure.Metrics;
using MetadataService.Models;
using MetadataService.Repositories;
using OpenTelemetry.Trace;

namespace MetadataService.Messaging.Consumers;

/// <summary>
/// Реагирует на событие FileProcessedV1 (аудио успешно загружено в хранилище).
/// </summary>
public sealed class FileProcessedConsumer : BaseConsumer<FileProcessedV1>
{
    private readonly IStorageClient     _storage;
    private readonly ISongRepository    _songRepo;
    private readonly IUnitOfWork        _uow;
    private readonly IBus               _bus; // чтобы отправить команду в очередь STT

    public FileProcessedConsumer(
        ILogger<BaseConsumer<FileProcessedV1>> log,
        Tracer tracer,
        IMetrics metrics,
        IStorageClient storage,
        ISongRepository songRepo,
        IUnitOfWork uow,
        IBus bus) : base(log, tracer, metrics)
    {
        _storage = storage;
        _songRepo = songRepo;
        _uow      = uow;
        _bus      = bus;
    }

    protected override async Task HandleAsync(ConsumeContext<FileProcessedV1> ctx)
    {
        var e = ctx.Message;
        // 1) скачиваем или проверяем наличие файла (можно опустить скачивание)
        await _storage.DownloadAsync(e.StorageKey, ctx.CancellationToken);

        // 2) отметим песню как processed
        var song = await _songRepo.GetByIdAsync(e.AudioFileId, ctx.CancellationToken)
                   ?? throw new InvalidOperationException($"Song {e.AudioFileId} not found");
        song.Status = SongStatus.Processed;
        await _uow.SaveChangesAsync(ctx.CancellationToken);

        // 3) публикуем задачу в очередь transcribe (может быть отдельный сервис)
        await _bus.Publish(new StartTranscriptionV1(e.AudioFileId, e.StorageKey), ctx.CancellationToken);
    }
}