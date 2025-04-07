using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Repositories;

namespace SmartLeadsPortalDotNetApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CallLogsController : ControllerBase
    {
        private readonly CallLogsRepository _callLogsRepository;
        public CallLogsController(CallLogsRepository callLogsRepository)
        {
            _callLogsRepository = callLogsRepository;
        }

        [HttpGet("get-lead-phone/{email}")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetleadContactNoByEmail(string email)
        {
            CallLogLeadNo? list = await _callLogsRepository.GetleadContactNoByEmail(email);
            return Ok(list);
        }
    }
}
