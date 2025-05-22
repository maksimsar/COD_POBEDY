using System.Text.Json;
using Common.Events;
using MassTransit;
using TranscriptionService.Adapters;
using TranscriptionService.DTOs;
using TranscriptionService.Interfaces;
using TranscriptionService.Messaging.Contracts;
using TranscriptionService.Models;
using TranscriptionService.Repositories;

namespace TranscriptionService.Services;

public sealed class TranscriptionService : ITranscriptionService
{
    private readonly IStorageClient _storage;                  
    private readonly ISttEngine     _sttEngine;                
    private readonly ITranscriptionJobRepository _repo;        
    private readonly IPublishEndpoint _bus;                    
    private readonly ILogger<TranscriptionService> _log;

    public TranscriptionService(
        IStorageClient               storage,
        ISttEngine                   sttEngine,
        ITranscriptionJobRepository  repo,
        IPublishEndpoint             bus,
        ILogger<TranscriptionService> log)
    {
        _storage    = storage;
        _sttEngine  = sttEngine;
        _repo       = repo;
        _bus        = bus;
        _log        = log;
    }
    
    public async Task ProcessAsync(long jobId,
                                   string storageKey,
                                   CancellationToken ct = default)
    {
        var job = await _repo.GetAsync(jobId, ct);
        if (job is null)
            throw new InvalidOperationException($"Job {jobId} not found");
        
        job.Status    = TranscriptionJob.JobStatus.Running;
        job.StartedAt = DateTimeOffset.UtcNow;
        await _repo.SaveAsync(ct);

        try
        {
            await using var wavStream = await _storage.DownloadAsync(storageKey, ct);
            
            var segments = await _sttEngine.RecognizeAsync(wavStream, job.Language, ct);
            
            job.Status      = TranscriptionJob.JobStatus.Done;
            job.FinishedAt  = DateTimeOffset.UtcNow;
            await _repo.SaveAsync(ct);
            
            var evt = new TranscriptReadyV2(
                job.AudioFileId,
                segments,
                null // TranscriptStorageKey, если не храните JSON в MinIO
            );

            var outbox = new OutboxMessage
            {
                Type          = typeof(TranscriptReadyV2).AssemblyQualifiedName!,
                Payload       = JsonSerializer.Serialize(evt),
                OccurredOnUtc = DateTimeOffset.UtcNow,
                Processed     = false
            };

            _repo.AddOutbox(outbox);
            await _repo.SaveAsync(ct);

            _log.LogInformation("Job {JobId} finished OK", jobId);
        }
        catch (Exception ex)
        {
            // 4b) статус → error
            job.Status       = TranscriptionJob.JobStatus.Error;
            job.ErrorMessage = ex.Message;
            await _repo.SaveAsync(ct);

            _log.LogError(ex, "Job {JobId} failed", jobId);
            throw;                              // пусть consumer решит, надо ли retry
        }
    }

    public async Task RetryAsync(long jobId, CancellationToken ct = default)
    {
        var job = await _repo.GetAsync(jobId, ct)
                  ?? throw new InvalidOperationException($"Job {jobId} not found");

        if (job.Status != TranscriptionJob.JobStatus.Error &&
            job.Status != TranscriptionJob.JobStatus.Done)
            throw new InvalidOperationException("Job is already in progress");
        
        job.Status      = TranscriptionJob.JobStatus.Queued;
        job.ErrorMessage = null;
        await _repo.SaveAsync(ct);
        
    }

    public async Task<TranscriptionJobDto?> GetAsync(long jobId, CancellationToken ct = default)
    {
        var job = await _repo.GetAsync(jobId, ct);
        return job is null
            ? null
            : new TranscriptionJobDto(
                  job.Id,
                  job.AudioFileId,
                  job.Status,
                  job.StartedAt,
                  job.FinishedAt,
                  job.ErrorMessage);
    }
}