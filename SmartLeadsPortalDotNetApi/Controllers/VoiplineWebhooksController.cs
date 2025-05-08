using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartLeadsPortalDotNetApi.Aggregates.InboundCall;
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
        private readonly InboundCallEventParser inboundCallEventParser;
        private readonly InboundCallRepository inboundCallRepository;
        private readonly IConfiguration configuration;

        public VoiplineWebhooksController(
            VoiplineWebhookRepository voiplineWebhookRepository, 
            OutboundCallRepository outboundCallRepository,
            OutboundEventStore outboundEventStore,
            OutboundCallEventParser outboundCallEventParser,
            InboundCallEventParser inboundCallEventParser,
            InboundCallRepository inboundCallRepository,
            IConfiguration configuration)
        {
            this.voiplineWebhookRepository = voiplineWebhookRepository;
            this.outboundCallRepository = outboundCallRepository;
            this.outboundEventStore = outboundEventStore;
            this.outboundCallEventParser = outboundCallEventParser;
            this.inboundCallEventParser = inboundCallEventParser;
            this.inboundCallRepository = inboundCallRepository;
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
            return Ok();
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

            await HandleIncomingCallPayload(payload);

            return Ok();
        }

        [HttpPost("user-inbound-call-answered")]
        public async Task<IActionResult> InboundCallAnswered()
        {
            var secret = this.configuration["VoiplineWebhook:Secret"];
            if (Request.Headers.TryGetValue("x-pbx-token", out var requestToken) && requestToken != secret)
            {
                return BadRequest("Invalid token");
            }

            using var reader = new StreamReader(Request.Body);
            string payload = await reader.ReadToEndAsync();
            await this.voiplineWebhookRepository.InsertWebhook("UserInboundCallAnswered", payload);

            await HandleIncomingCallPayload(payload);
            return Ok();
        }

        [HttpPost("user-inbound-call-completed")]
        public async Task<IActionResult> InboundCallCompletion()
        {
            var secret = this.configuration["VoiplineWebhook:Secret"];
            if (Request.Headers.TryGetValue("x-pbx-token", out var requestToken) && requestToken != secret)
            {
                return BadRequest("Invalid token");
            }

            using var reader = new StreamReader(Request.Body);
            string payload = await reader.ReadToEndAsync();
            await this.voiplineWebhookRepository.InsertWebhook("UserInboundCallCompleted", payload);

            await HandleIncomingCallPayload(payload);
            return Ok();
        }

        [HttpPost("queue-call")]
        public async Task<IActionResult> QueueCallSummary()
        {
            var secret = this.configuration["VoiplineWebhook:Secret"];
            if (Request.Headers.TryGetValue("x-pbx-token", out var requestToken) && requestToken != secret)
            {
                return BadRequest("Invalid token");
            }

            using var reader = new StreamReader(Request.Body);
            string payload = await reader.ReadToEndAsync();
            await this.voiplineWebhookRepository.InsertWebhook("QueueCall", payload);

            await HandleIncomingCallPayload(payload);
            return Ok();
        }

        [HttpPost("ring-group-call")]
        public async Task<IActionResult> RingGroupCallSummary()
        {
            var secret = this.configuration["VoiplineWebhook:Secret"];
            if (Request.Headers.TryGetValue("x-pbx-token", out var requestToken) && requestToken != secret)
            {
                return BadRequest("Invalid token");
            }

            using var reader = new StreamReader(Request.Body);
            string payload = await reader.ReadToEndAsync();
            await this.voiplineWebhookRepository.InsertWebhook("RingGroupCall", payload);

            await HandleIncomingCallPayload(payload);
            return Ok();
        }

        [HttpPost("voicemail")]
        public async Task<IActionResult> Voicemail()
        {
            var secret = this.configuration["VoiplineWebhook:Secret"];
            if (Request.Headers.TryGetValue("x-pbx-token", out var requestToken) && requestToken != secret)
            {
                return BadRequest("Invalid token");
            }

            using var reader = new StreamReader(Request.Body);
            string payload = await reader.ReadToEndAsync();
            await this.voiplineWebhookRepository.InsertWebhook("Voicemail", payload);

            await HandleIncomingCallPayload(payload);
            return Ok();
        }

        [HttpPost("recording-inbound")]
        public async Task<IActionResult> InboundCallRecording()
        {
            var secret = this.configuration["VoiplineWebhook:Secret"];
            if (Request.Headers.TryGetValue("x-pbx-token", out var requestToken) && requestToken != secret)
            {
                return BadRequest("Invalid token");
            }

            using var reader = new StreamReader(Request.Body);
            string payload = await reader.ReadToEndAsync();
            await this.voiplineWebhookRepository.InsertWebhook("RecordingInbound", payload);

            await HandleIncomingCallPayload(payload);
            return Ok();
        }

        private async Task HandleIncomingCallPayload(string payload)
        {
            var inboundCallEvent = inboundCallEventParser.ParseEvent(payload);
            var inboundCall = await this.inboundCallRepository.GetInboundCallAggregate(inboundCallEvent.UniqueCallId);
            inboundCall.ApplyEvent(inboundCallEvent);
            await this.inboundCallRepository.UpsertInboundCallAggregate(inboundCall);
        }

        [HttpPost("process-outbound-call/{uniqueCallId}")]
        public async Task<IActionResult> ProcessCall(string uniqueCallId)
        {

            var callAggregate = await this.outboundEventStore.GetOutboundCallAggregate(uniqueCallId);
            await this.outboundCallRepository.UpsertOutboundCallAggregate(callAggregate);
            return Ok();
        }
    }
}
