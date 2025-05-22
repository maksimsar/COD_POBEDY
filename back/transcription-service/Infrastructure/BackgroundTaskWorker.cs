using TranscriptionService.Interfaces;

namespace TranscriptionService.Infrastructure;

public sealed class BackgroundTaskWorker : BackgroundService
{
    private readonly IBackgroundTaskQueue    _queue;
    private readonly ILogger<BackgroundTaskWorker> _log;

    public BackgroundTaskWorker(IBackgroundTaskQueue queue,
        ILogger<BackgroundTaskWorker> log)
    {
        _queue = queue;
        _log   = log;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _log.LogInformation("BackgroundTaskWorker started");
        while (!stoppingToken.IsCancellationRequested)
        {
            var workItem = await _queue.DequeueAsync(stoppingToken);
            try
            {
                await workItem(stoppingToken);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Background task failed");
                // решайте, нужен ли retry / DLQ
            }
        }
        _log.LogInformation("BackgroundTaskWorker stopped");
    }
}