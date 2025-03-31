using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartLeadsPortalDotNetApi.Repositories;

namespace SmartLeadsPortalDotNetApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeadClicksController : ControllerBase
    {
        private readonly LeadClicksRepository leadClicksRepository;
        private readonly WebhooksRepository webhooksRepository;

        public LeadClicksController(LeadClicksRepository leadClicksRepository, WebhooksRepository webhooksRepository)
        {
            this.leadClicksRepository = leadClicksRepository;
            this.webhooksRepository = webhooksRepository;
        }

        [HttpGet()]
        public async Task<IActionResult> GetLeadClicks()
        {
            try
            {
                var result = await leadClicksRepository.GetLeadClicks();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("from-webhook")]
        public async Task<IActionResult> GetLeadClickFromWebhook()
        {
            try
            {
                var result = await webhooksRepository.GetLeadClick();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
