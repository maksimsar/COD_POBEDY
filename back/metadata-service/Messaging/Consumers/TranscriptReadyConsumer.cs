using Common.Events;
using MassTransit;
using MetadataService.Infrastructure.Metrics;
using MetadataService.Repositories;
using MetadataService.Models;
using OpenTelemetry.Trace;

namespace MetadataService.Messaging.Consumers;

/// <summary>
/// Получает готовые сегменты транскрипта, сохраняет в БД и бросает SongUpdatedV1 в outbox.
/// </summary>
public sealed class TranscriptReadyConsumer : BaseConsumer<TranscriptReadyV1>
{
    private readonly ITranscriptRepository _trRepo;
    private readonly ISongRepository _songRepo;
    private readonly IUnitOfWork _uow;
    private readonly IPublishEndpoint _bus;

    public TranscriptReadyConsumer(
        ILogger<BaseConsumer<TranscriptReadyV1>> log,
        Tracer tracer,
        IMetrics metrics,
        ITranscriptRepository trRepo,
        ISongRepository songRepo,
        IUnitOfWork uow,
        IPublishEndpoint bus)
        : base(log, tracer, metrics)
    {
        _trRepo = trRepo;
        _songRepo = songRepo;
        _uow = uow;
        _bus = bus;
    }

    protected override async Task HandleAsync(ConsumeContext<TranscriptReadyV1> ctx)
    {
        var e = ctx.Message;
        // 1) сохранить сегменты
        foreach (var seg in e.Segments)
        {
            _trRepo.Add(new Transcript
            {
                SongId       = e.SongId,
                SegmentIndex = seg.Index,
                StartMs      = seg.StartMs,
                EndMs        = seg.EndMs,
                Text         = seg.Text,
                Confidence   = (decimal)seg.Confidence
            });
        }

        var song = await _songRepo.GetByIdAsync(e.SongId, ctx.CancellationToken)
                   ?? throw new InvalidOperationException($"Song {e.SongId} not found");
        song.Status = SongStatus.Transcribed;

        // 2) Outbox событие
        await _bus.Publish(new SongUpdatedV1(song.Id), ctx.CancellationToken);

        await _uow.SaveChangesAsync(ctx.CancellationToken);
    }
}
