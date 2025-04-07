using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Repositories;

namespace SmartLeadsPortalDotNetApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CallStateController : ControllerBase
    {
        private readonly CallStateRepository _callStateRepository;
        public CallStateController(CallStateRepository callStateRepository)
        {
            _callStateRepository = callStateRepository;
        }

        [HttpPost("insert-callstate")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> InsertCallState([FromBody] CallStateInsert request)
        {
            if (string.IsNullOrEmpty(request.StateName))
            {
                return BadRequest(new { error = "Call State Name text is required." });
            }

            await _callStateRepository.InsertCallState(request);
            return Ok(new { message = "Call State Name created successfully." });
        }

        [HttpPost("update-callstate")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> UpdateCallState([FromBody] CallState request)
        {
            if (string.IsNullOrEmpty(request.StateName))
            {
                return BadRequest(new { error = "Call State Name text is required." });
            }

            await _callStateRepository.UpdateCallState(request);
            return Ok(new { message = "Call State Name created successfully." });
        }

        [HttpPost("get-all-callstate-list")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetAllCallStateList(ExcludedKeywordsListRequest param)
        {
            CallStateResponseModel<CallState> list = await _callStateRepository.GetAllCallStateList(param);
            return Ok(list);
        }

        [HttpGet("get-callstate-id/{guid}")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetCallStateById(Guid guid)
        {
            CallState? list = await _callStateRepository.GetCallStateById(guid);
            return Ok(list);
        }

        [HttpGet("get-callstate-retrieveall")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetCallStateRetrieveAll()
        {
            IEnumerable<CallState>? list = await _callStateRepository.GetCallStateRetrievedAll();
            return Ok(list);
        }

        [HttpGet("delete-callstate/{guid}")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> DeleteCallState(Guid guid)
        {
            await _callStateRepository.DeleteCallState(guid);
            return Ok(new { message = "Call State deleted successfully." });
        }
    }
}
