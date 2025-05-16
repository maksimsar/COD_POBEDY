using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using MetadataService.Messaging.Contracts;
using MetadataService.Repositories;
using MetadataService.Application.Services;
using MetadataService.Models;

namespace MetadataService.Messaging.Consumers;

/// <summary>
/// Получает готовые сегменты транскрипта, сохраняет в БД и бросает SongUpdatedV1 в outbox.
/// </summary>
public sealed class TranscriptReadyConsumer : BaseConsumer<TranscriptReadyV1>
{
    private readonly ITranscriptRepository      _trRepo;
    private readonly ISongRepository            _songRepo;
    private readonly IUnitOfWork                _uow;
    private readonly IOutboxPublisher           _outbox;

    public TranscriptReadyConsumer(
        ILogger<BaseConsumer<TranscriptReadyV1>> log,
        Tracer tracer,
        IMetrics metrics,
        ITranscriptRepository trRepo,
        ISongRepository songRepo,
        IUnitOfWork uow,
        IOutboxPublisher outbox)
        : base(log, tracer, metrics)
    {
        _trRepo = trRepo;
        _songRepo = songRepo;
        _uow = uow;
        _outbox = outbox;
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
                Confidence   = seg.Confidence
            });
        }

        var song = await _songRepo.GetByIdAsync(e.SongId, ctx.CancellationToken)
                   ?? throw new InvalidOperationException($"Song {e.SongId} not found");
        song.Status = SongStatus.Transcribed;

        // 2) Outbox событие
        _outbox.Add(new SongUpdatedV1(song.Id));

        await _uow.SaveChangesAsync(ctx.CancellationToken);
    }
}
