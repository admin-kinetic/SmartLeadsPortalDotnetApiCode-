using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartLeadsPortalDotNetApi.Aggregates.OutboundCall;
using SmartLeadsPortalDotNetApi.Repositories;

namespace SmartLeadsPortalDotNetApi.Controllers
{
    [Route("api/voipline-webhooks")]
    [ApiController]
    public class VoiplineWebhooksController : ControllerBase
    {
        private readonly VoiplineWebhookRepository voiplineWebhookRepository;
        private readonly OutboundCallRepository outboundCallRepository;
        private readonly OutboundEventStore outboundEventStore;
        private readonly OutboundCallEventParser outboundCallEventParser;
        private readonly IConfiguration configuration;

        public VoiplineWebhooksController(
            VoiplineWebhookRepository voiplineWebhookRepository, 
            OutboundCallRepository outboundCallRepository,
            OutboundEventStore outboundEventStore,
            OutboundCallEventParser outboundCallEventParser,
            IConfiguration configuration)
        {
            this.voiplineWebhookRepository = voiplineWebhookRepository;
            this.outboundCallRepository = outboundCallRepository;
            this.outboundEventStore = outboundEventStore;
            this.outboundCallEventParser = outboundCallEventParser;
            this.configuration = configuration;
        }

        [HttpPost("user-outbound-call")]
        public async Task<IActionResult> OutboundCall()
        {
            var secret = this.configuration["VoiplineWebhook:Secret"];
            if (Request.Headers.TryGetValue("x-pbx-token", out var requestToken) && requestToken != secret)
            {
                return BadRequest("Invalid token");
            }

            using var reader = new StreamReader(Request.Body);
            string payload = await reader.ReadToEndAsync();
            await this.voiplineWebhookRepository.InsertWebhook("UserOutboundCall", payload);

            var userOutboundCall = outboundCallEventParser.ParseEvent(payload);
            var outboundCall = await this.outboundCallRepository.GetOutboundCallAggregate(userOutboundCall.UniqueCallId);
            outboundCall.ApplyEvent(userOutboundCall);            
            await this.outboundCallRepository.UpsertOutboundCallAggregate(outboundCall);
            return Ok(outboundCall);
        }

        [HttpPost("user-outbound-call-answered")]
        public async Task<IActionResult> OutboundCallAnswered()
        {
            using var reader = new StreamReader(Request.Body);
            string payload = await reader.ReadToEndAsync();
            await this.voiplineWebhookRepository.InsertWebhook("UserOutboundCallAnswered", payload);


            var userOutboundCallAnswered = outboundCallEventParser.ParseEvent(payload);
            var outboundCall = await this.outboundCallRepository.GetOutboundCallAggregate(userOutboundCallAnswered.UniqueCallId);
            outboundCall.ApplyEvent(userOutboundCallAnswered);            
            await this.outboundCallRepository.UpsertOutboundCallAggregate(outboundCall);
            return Ok();
        }

        [HttpPost("user-outbound-call-completion")]
        public async Task<IActionResult> OutboundCallCompletion()
        {
            using var reader = new StreamReader(Request.Body);
            string payload = await reader.ReadToEndAsync();
            await this.voiplineWebhookRepository.InsertWebhook("UserOutboundCallCompletion", payload);

            var userOutboundCallCompletion = outboundCallEventParser.ParseEvent(payload);
            var outboundCall = await this.outboundCallRepository.GetOutboundCallAggregate(userOutboundCallCompletion.UniqueCallId);
            outboundCall.ApplyEvent(userOutboundCallCompletion);            
            await this.outboundCallRepository.UpsertOutboundCallAggregate(outboundCall);
            return Ok();
        }

        [HttpPost("outbound-call-recording")]
        public async Task<IActionResult> OutboundCallRecording()
        {
            using var reader = new StreamReader(Request.Body);
            string payload = await reader.ReadToEndAsync();
            await this.voiplineWebhookRepository.InsertWebhook("OutboundCallRecording", payload);

            var outboundCallRecording = outboundCallEventParser.ParseEvent(payload);
            var outboundCall = await this.outboundCallRepository.GetOutboundCallAggregate(outboundCallRecording.UniqueCallId);
            outboundCall.ApplyEvent(outboundCallRecording);            
            await this.outboundCallRepository.UpsertOutboundCallAggregate(outboundCall);
            return Ok();
        }

        [HttpPost("user-inbound-call")]
        public async Task<IActionResult> InboundCall()
        {
            var secret = this.configuration["VoiplineWebhook:Secret"];
            if (Request.Headers.TryGetValue("x-pbx-token", out var requestToken) && requestToken != secret)
            {
                return BadRequest("Invalid token");
            }

            using var reader = new StreamReader(Request.Body);
            string payload = await reader.ReadToEndAsync();
            await this.voiplineWebhookRepository.InsertWebhook("UserInboundCall", payload);

            var userOutboundCall = outboundCallEventParser.ParseEvent(payload);
            var outboundCall = await this.outboundCallRepository.GetOutboundCallAggregate(userOutboundCall.UniqueCallId);
            outboundCall.ApplyEvent(userOutboundCall);
            await this.outboundCallRepository.UpsertOutboundCallAggregate(outboundCall);
            return Ok(outboundCall);
        }

        [HttpPost("user-inbound-call-answered")]
        public async Task<IActionResult> InboundCallAnswered()
        {
            using var reader = new StreamReader(Request.Body);
            string payload = await reader.ReadToEndAsync();
            await this.voiplineWebhookRepository.InsertWebhook("UserInboundCallAnswered", payload);


            var userOutboundCallAnswered = outboundCallEventParser.ParseEvent(payload);
            var outboundCall = await this.outboundCallRepository.GetOutboundCallAggregate(userOutboundCallAnswered.UniqueCallId);
            outboundCall.ApplyEvent(userOutboundCallAnswered);
            await this.outboundCallRepository.UpsertOutboundCallAggregate(outboundCall);
            return Ok();
        }

        [HttpPost("user-inbound-call-completion")]
        public async Task<IActionResult> InboundCallCompletion()
        {
            using var reader = new StreamReader(Request.Body);
            string payload = await reader.ReadToEndAsync();
            await this.voiplineWebhookRepository.InsertWebhook("UserInbountCallCompletion", payload);

            var userOutboundCallCompletion = outboundCallEventParser.ParseEvent(payload);
            var outboundCall = await this.outboundCallRepository.GetOutboundCallAggregate(userOutboundCallCompletion.UniqueCallId);
            outboundCall.ApplyEvent(userOutboundCallCompletion);
            await this.outboundCallRepository.UpsertOutboundCallAggregate(outboundCall);
            return Ok();
        }

        [HttpPost("queue-call-summary")]
        public async Task<IActionResult> QueueCallSummary()
        {
            using var reader = new StreamReader(Request.Body);
            string payload = await reader.ReadToEndAsync();
            await this.voiplineWebhookRepository.InsertWebhook("QueueCallSummary", payload);

            var outboundCallRecording = outboundCallEventParser.ParseEvent(payload);
            var outboundCall = await this.outboundCallRepository.GetOutboundCallAggregate(outboundCallRecording.UniqueCallId);
            outboundCall.ApplyEvent(outboundCallRecording);
            await this.outboundCallRepository.UpsertOutboundCallAggregate(outboundCall);
            return Ok();
        }

        [HttpPost("ring-group-call-summary")]
        public async Task<IActionResult> RingGroupCallSummary()
        {
            using var reader = new StreamReader(Request.Body);
            string payload = await reader.ReadToEndAsync();
            await this.voiplineWebhookRepository.InsertWebhook("RingGroupCallSummary", payload);

            var outboundCallRecording = outboundCallEventParser.ParseEvent(payload);
            var outboundCall = await this.outboundCallRepository.GetOutboundCallAggregate(outboundCallRecording.UniqueCallId);
            outboundCall.ApplyEvent(outboundCallRecording);
            await this.outboundCallRepository.UpsertOutboundCallAggregate(outboundCall);
            return Ok();
        }

        [HttpPost("voicemail")]
        public async Task<IActionResult> Voicemail()
        {
            using var reader = new StreamReader(Request.Body);
            string payload = await reader.ReadToEndAsync();
            await this.voiplineWebhookRepository.InsertWebhook("Voicemail", payload);

            var outboundCallRecording = outboundCallEventParser.ParseEvent(payload);
            var outboundCall = await this.outboundCallRepository.GetOutboundCallAggregate(outboundCallRecording.UniqueCallId);
            outboundCall.ApplyEvent(outboundCallRecording);
            await this.outboundCallRepository.UpsertOutboundCallAggregate(outboundCall);
            return Ok();
        }

        [HttpPost("inbound-call-recording")]
        public async Task<IActionResult> InboundCallRecording()
        {
            using var reader = new StreamReader(Request.Body);
            string payload = await reader.ReadToEndAsync();
            await this.voiplineWebhookRepository.InsertWebhook("InboundCallRecording", payload);

            var outboundCallRecording = outboundCallEventParser.ParseEvent(payload);
            var outboundCall = await this.outboundCallRepository.GetOutboundCallAggregate(outboundCallRecording.UniqueCallId);
            outboundCall.ApplyEvent(outboundCallRecording);
            await this.outboundCallRepository.UpsertOutboundCallAggregate(outboundCall);
            return Ok();
        }
    }
}
