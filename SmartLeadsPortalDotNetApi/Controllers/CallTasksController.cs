using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartLeadsPortalDotNetApi.Services;

namespace SmartLeadsPortalDotNetApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CallTasksController : ControllerBase
    {
        private readonly LeadsPortalHttpService leadsPortalHttpService;

        public CallTasksController(LeadsPortalHttpService leadsPortalHttpService)
        {
            this.leadsPortalHttpService = leadsPortalHttpService;
        }

        [HttpGet("contact-details/{email}")]
        public async Task<IActionResult> GetContactDetailsByEmail(string email)
        {
            try
            {
                var contactDetails = await this.leadsPortalHttpService.GetContactDetailsByEmail(email);
                return Ok(contactDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
