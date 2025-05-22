using System.Threading.Channels;
using TranscriptionService.Interfaces;

namespace TranscriptionService.Infrastructure;

public sealed class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly Channel<Func<CancellationToken, Task>> _channel;

    public BackgroundTaskQueue(int capacity = 100)
    {
        // Channel = неблокирующая очередь из BCL
        _channel = Channel.CreateBounded<Func<CancellationToken, Task>>(new BoundedChannelOptions(capacity)
        {
            SingleReader = true,
            SingleWriter = false
        });
    }

    public void Enqueue(Func<CancellationToken, Task> workItem)
    {
        if (workItem is null) throw new ArgumentNullException(nameof(workItem));
        if (!_channel.Writer.TryWrite(workItem))
            throw new InvalidOperationException("Queue capacity reached");
    }

    public ValueTask<Func<CancellationToken, Task>> DequeueAsync(CancellationToken ct) =>
        _channel.Reader.ReadAsync(ct);
}