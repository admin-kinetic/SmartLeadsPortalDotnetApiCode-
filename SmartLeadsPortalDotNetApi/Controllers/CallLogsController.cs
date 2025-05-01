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
        public async Task<IActionResult> InsertCallLogs([FromBody] CallsInsert request)
        {
            await _callLogsRepository.InsertCallLogs(request);
            return Ok(new { message = "call logs created successfully." });
        }

        [HttpPost("insert-call-logs-inbound")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> InsertInboundCallLogs([FromBody] CallsInsert request)
        {
            await _callLogsRepository.InsertInboundCallLogs(request);
            return Ok(new { message = "call logs created successfully." });
        }

        [HttpPost("update-call-logs")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> UpdateCallLogs([FromBody] CallsUpdate request)
        {
            await _callLogsRepository.UpdateCallLogs(request);
            return Ok(new { message = "call logs created successfully." });
        }

        [HttpPost("delete-call-logs")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> DeleteCallLogs([FromBody] CallsUpdate request)
        {
            var ret = await _callLogsRepository.DeleteCallLogs(request);
            return Ok(ret);
        }

    }
}
