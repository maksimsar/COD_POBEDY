using MassTransit;
using ProcessingService.DTOs;
using ProcessingService.Utils;
using System.Threading.Tasks;

namespace ProcessingService.Services
{
    public class DenoiseConsumer : IConsumer<DenoiseRequest>
    {
        readonly IMlServiceClient _ml;
        readonly IStorageService _storage;
        readonly IPublishEndpoint _publish;

        public DenoiseConsumer(IMlServiceClient ml, IStorageService storage, IPublishEndpoint publish)
        {
            _ml = ml;
            _storage = storage;
            _publish = publish;
        }

        public async Task Consume(ConsumeContext<DenoiseRequest> ctx)
        {
            var msg = ctx.Message;
            var original = await _storage.DownloadAsync(msg.OriginalUrl);
            var processed = await _ml.DenoiseAsync(original);
            var url = await _storage.SaveProcessedAsync(msg.RecordId, processed);

            await _publish.Publish(new DenoiseResult
            {
                RecordId = msg.RecordId,
                ProcessedUrl = url
            });
        }
    }
}
