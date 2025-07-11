using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartLeadsPortalDotNetApi.Entities;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Repositories;

namespace SmartLeadsPortalDotNetApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly CallsReportRepository _callsReportRepository;
        public ReportsController(CallsReportRepository callsReportRepository)
        {
            _callsReportRepository = callsReportRepository;
        }

        [HttpPost("get-all-calls")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetAllCalls(CallsParam request)
        {
            var list = await _callsReportRepository.GetAllCalls(request);
            return Ok(list);
        }

        [HttpPost("get-all-calls-count")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetAllCallsCount(CallsParam request)
        {
            var list = await _callsReportRepository.GetAllCallsCount(request);
            return Ok(list);
        }

        [HttpGet("get-bdr-calls-dropdown")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetBdrCalls()
        {
            var list = await _callsReportRepository.GetBdrCalls();
            return Ok(list);
        }
    }
}
