using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartLeadsPortalDotNetApi.Aggregates.OutboundCall;
using SmartLeadsPortalDotNetApi.Repositories;

namespace SmartLeadsPortalDotNetApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OutboundCallController : ControllerBase
    {
        private readonly OutboundEventStore outboundEventStore;
        private readonly VoiplineWebhookRepository voiplineWebhookRepository;
        private readonly OutboundCallRepository outboundCallRepository;

        public OutboundCallController(
            OutboundEventStore outboundEventStore,
            VoiplineWebhookRepository voiplineWebhookRepository,
            OutboundCallRepository outboundCallRepository)
        {
            this.outboundEventStore = outboundEventStore;
            this.voiplineWebhookRepository = voiplineWebhookRepository;
            this.outboundCallRepository = outboundCallRepository;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var outboundCall = await outboundEventStore.GetOutboundCallAggregate(id);
            return Ok(outboundCall);
        }

        [HttpPost("process")]
        public async Task<IActionResult> ProcessAllEvents()
        {
            var uniqueCallIds = await voiplineWebhookRepository.GetAllUniqueCallId();
            foreach (var id in uniqueCallIds)
            {
                var outboundCall = await outboundEventStore.GetOutboundCallAggregate(id);
                await outboundCallRepository.UpsertOutboundCallAggregate(outboundCall);
            }
            return Ok();
        }

        [HttpPost("process/{id}")]
        public async Task<IActionResult> ProcessAllEvents(string id)
        {
            var outboundCall = await outboundEventStore.GetOutboundCallAggregate(id);
            await outboundCallRepository.UpsertOutboundCallAggregate(outboundCall);
            return Ok();
        }
    }
}
