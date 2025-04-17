using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartLeadsPortalDotNetApi.Entities;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Repositories;
using SmartLeadsPortalDotNetApi.Services;

namespace SmartLeadsPortalDotNetApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CallLogsController : ControllerBase
    {
        private readonly CallLogsRepository _callLogsRepository;
        private readonly LeadsPortalHttpService leadsPortalHttpService;

        public CallLogsController(CallLogsRepository callLogsRepository, LeadsPortalHttpService leadsPortalHttpService)
        {
            _callLogsRepository = callLogsRepository;
            this.leadsPortalHttpService = leadsPortalHttpService;
        }

        [HttpGet("get-lead-phone/{email}")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetleadContactNoByEmail(string email)
        {
            var list = await this.leadsPortalHttpService.GetContactDetailsByEmail(email);
            return Ok(list);
        }

        [HttpPost("insert-call-logs")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> InsertCallTags([FromBody] CallsInsert request)
        {
            await _callLogsRepository.InsertCallLogs(request);
            return Ok(new { message = "call logs created successfully." });
        }

        [HttpPost("update-call-logs")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> UpdateCallLogs([FromBody] CallsUpdate request)
        {
            await _callLogsRepository.UpdateCallLogs(request);
            return Ok(new { message = "call logs created successfully." });
        }

    }
}
