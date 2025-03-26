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

        public LeadClicksController(LeadClicksRepository leadClicksRepository)
        {
            this.leadClicksRepository = leadClicksRepository;
        }

        [HttpGet]
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
    }
}
