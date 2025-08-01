using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartLeadsPortalDotNetApi.Entities;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Repositories;
using SmartLeadsPortalDotNetApi.Services;

namespace SmartLeadsPortalDotNetApi.Controllers
{
    [Authorize]
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
            //var list = await this.leadsPortalHttpService.GetContactDetailsByEmail(email);
            var list = await _callLogsRepository.GetleadContactNoByEmail(email);
            return Ok(list);
        }

        [HttpPost("insert-call-logs")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> InsertCallLogs([FromBody] CallsInsert request)
        {
            var response = await _callLogsRepository.InsertCallLogs(request);

            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("upsert-call-logs-from-lead-details")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> UpsertCallLogsFromLeadDetails([FromBody] CallsUpsert request)
        {
            var response = await _callLogsRepository.UpsertCallLogsFromLeadDetails(request);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        [HttpPost("insert-call-logs-inbound")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> InsertInboundCallLogs([FromBody] CallsInsertInbound request)
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

        [HttpPost("get-user-by-phoneno")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetEmployeeNameByPhonenumber(CallLogLeadNo param)
        {
            var list = await _callLogsRepository.GetEmployeeNameByPhonenumber(param);
            return Ok(list);
        }

    }
}
