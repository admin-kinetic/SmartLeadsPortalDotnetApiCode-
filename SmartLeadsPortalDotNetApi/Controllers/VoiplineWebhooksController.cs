using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartLeadsPortalDotNetApi.Repositories;

namespace SmartLeadsPortalDotNetApi.Controllers
{
    [Route("api/voipline-webhooks")]
    [ApiController]
    public class VoiplineWebhooksController : ControllerBase
    {
        private readonly VoiplineWebhookRepository voiplineWebhookRepository;

        public VoiplineWebhooksController(VoiplineWebhookRepository voiplineWebhookRepository)
        {
            this.voiplineWebhookRepository = voiplineWebhookRepository;
        }

        [HttpPost("user-outbound-call")]
        public async Task<IActionResult> OutboundCall([FromBody] Dictionary<string, object> payload)
        {
            await this.voiplineWebhookRepository.InsertWebhook("UserOutboundCall", JsonSerializer.Serialize(payload));
            return Ok();
        }

        [HttpPost("user-outbound-call-answered")]
        public async Task<IActionResult> OutboundCallAnswered([FromBody] Dictionary<string, object> payload)
        {
            await this.voiplineWebhookRepository.InsertWebhook("UserOutboundCallAnswered", JsonSerializer.Serialize(payload));
            return Ok();
        }

        [HttpPost("user-outbound-call-completion")]
        public async Task<IActionResult> OutboundCallCompletion([FromBody] Dictionary<string, object> payload)
        {
            await this.voiplineWebhookRepository.InsertWebhook("UserOutboundCallCompletion", JsonSerializer.Serialize(payload));
            return Ok();
        }

        [HttpPost("outbound-call-recording")]
        public async Task<IActionResult> OutboundCallRecording([FromBody] Dictionary<string, object> payload)
        {
            await this.voiplineWebhookRepository.InsertWebhook("OutboundCall", JsonSerializer.Serialize(payload));
            return Ok();
        }
    }
}
