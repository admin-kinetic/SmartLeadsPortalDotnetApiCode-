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

        [HttpPost("outbound-call")]
        public async Task<IActionResult> OutboundCall([FromBody] Dictionary<string, object> payload)
        {
            await this.voiplineWebhookRepository.InsertWebhook(JsonSerializer.Serialize(payload));
            return Ok();
        }
    }
}
