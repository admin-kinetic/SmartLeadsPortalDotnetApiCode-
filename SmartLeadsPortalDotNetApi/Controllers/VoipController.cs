using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartLeadsPortalDotNetApi.Services;
using SmartLeadsPortalDotNetApi.Services.Model;

namespace SmartLeadsPortalDotNetApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class VoipController : ControllerBase
    {
        private readonly VoipHttpService _voipHttpService;
        public VoipController(VoipHttpService voipHttpService)
        {
            _voipHttpService = voipHttpService;
        }

        [HttpGet("get-user-calls")]
        public async Task<IActionResult> GetVoipData()
        {
            try
            {
                var voipData = await _voipHttpService.GetVoipData();
                return Ok(voipData);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("get-users-calls")]
        public async Task<IActionResult> GetUsersCalls([FromQuery] string sortBy = "newest_first", string? fromDate = null, string? toDate = null, int offset = 0, int limit = 100, string? uniqueCallId = null)
        {
            try
            {
                var voipData = await _voipHttpService.GetUsersCalls(sortBy, fromDate, toDate, offset, limit, uniqueCallId);
                return Ok(voipData);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("queue-calls")]
        public async Task<IActionResult> GetQueueCalls([FromQuery] string sortBy = "newest_first", string? fromDate = null, string? toDate = null, string[]? queues = null, int offset = 0, int limit = 100, string? uniqueCallId = null)
        {
            try
            {
                var voipData = await _voipHttpService.GetQueueCalls(sortBy, fromDate, toDate, queues, offset, limit, uniqueCallId);
                return Ok(voipData);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("ring-groups-calls")]
        public async Task<IActionResult> GetRingGroupCalls([FromQuery] string sortBy = "newest_first", string? fromDate = null, string? toDate = null, string? ringGroups = null, int offset = 0, int limit = 100, string? uniqueCallId = null)
        {
            try
            {
                var voipData = await _voipHttpService.GetRingGroupCalls(sortBy, fromDate, toDate, ringGroups, offset, limit, uniqueCallId);
                return Ok(voipData);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("initiate-call")]
        public async Task<IActionResult> InitiateCall([FromBody] CallToNumberRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.User) || string.IsNullOrEmpty(request.NumberToCall))
                {
                    return BadRequest("User and NumberToCall are required fields");
                }

                var response = await _voipHttpService.InitiateCallToNumber(request);
                return Ok(response);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status502BadGateway, new
                {
                    Status = "error",
                    Message = ex.Message
                });
            }
        }
    }
}
