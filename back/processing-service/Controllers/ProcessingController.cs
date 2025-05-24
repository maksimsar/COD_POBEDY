using MassTransit;
using Microsoft.AspNetCore.Mvc;
using ProcessingService.DTOs;
using System.Threading.Tasks;

namespace ProcessingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProcessingController : ControllerBase
    {
        readonly IPublishEndpoint _publisher;
        public ProcessingController(IPublishEndpoint publisher) => _publisher = publisher;

        [HttpPost("denoise")]
        public async Task<IActionResult> EnqueueDenoise([FromBody] DenoiseRequest req)
        {
            await _publisher.Publish(req);
            return Accepted();
        }
    }
}
