using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Repositories;

namespace SmartLeadsPortalDotNetApi.Controllers
{
    [Route("api/leads-portal-webhook")]
    [ApiController]
    public class LeadsPortalWebhookController : ControllerBase
    {
        private readonly SmartLeadsExportedContactsRepository smartLeadsExportedContactsRepository;
        private readonly LeadsPortalWebhookRepository leadsPortalWebhookRepository;
        private readonly ILogger<LeadsPortalWebhookController> logger;

        public LeadsPortalWebhookController(
            SmartLeadsExportedContactsRepository smartLeadsExportedContactsRepository, 
            LeadsPortalWebhookRepository leadsPortalWebhookRepository,
            ILogger<LeadsPortalWebhookController> logger)
        {
            this.smartLeadsExportedContactsRepository = smartLeadsExportedContactsRepository;
            this.leadsPortalWebhookRepository = leadsPortalWebhookRepository;
            this.logger = logger;
        }

        [HttpPost("exported-contacts")]
        public async Task<IActionResult> ExportedContacts()
        {
            this.logger.LogInformation("Processing exported contacts");
            using var reader = new StreamReader(Request.Body);
            string payload = await reader.ReadToEndAsync();

            await this.leadsPortalWebhookRepository.Insert(payload, "EXPORTED_CONTACTS");

            var exportedContactsPayload = JsonSerializer.Deserialize<List<ExportedContactsPayload>>(payload);
            this.logger.LogInformation($"Recieved {exportedContactsPayload?.Count()} exported contacts");

            await this.smartLeadsExportedContactsRepository.SaveExportedContacts(exportedContactsPayload);
            return Ok();
        }
    }
}
