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

        public LeadsPortalWebhookController(SmartLeadsExportedContactsRepository smartLeadsExportedContactsRepository)
        {
            this.smartLeadsExportedContactsRepository = smartLeadsExportedContactsRepository;
        }

        [HttpPost("exported-contacts")]
        public async Task<IActionResult> ExportedContacts()
        {
            using var reader = new StreamReader(Request.Body);
            string payload = await reader.ReadToEndAsync();

            var exportedContactsPayload = JsonSerializer.Deserialize<List<ExportedContactsPayload>>(payload);

            await this.smartLeadsExportedContactsRepository.SaveExportedContacts(exportedContactsPayload);
            return Ok();
        }
    }
}
