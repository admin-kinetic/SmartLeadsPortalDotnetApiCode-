using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Repositories;

namespace SmartLeadsPortalDotNetApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CallPurposeController : ControllerBase
    {
        private readonly CallPurposeRepository _callPurposeRepository;
        public CallPurposeController(CallPurposeRepository callPurposeRepository)
        {
            _callPurposeRepository = callPurposeRepository;
        }

        [HttpPost("insert-callpurpose")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> InsertCallPurpose([FromBody] CallPurposeInsert request)
        {
            if (string.IsNullOrEmpty(request.CallPurposeName))
            {
                return BadRequest(new { error = "Call Purpose Name text is required." });
            }

            await _callPurposeRepository.InsertCallPurpose(request);
            return Ok(new { message = "Call Purpose Name created successfully." });
        }

        [HttpPost("update-callpurpose")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> UpdateCallPurpose([FromBody] CallPurpose request)
        {
            if (string.IsNullOrEmpty(request.CallPurposeName))
            {
                return BadRequest(new { error = "Call Purpose Name text is required." });
            }

            await _callPurposeRepository.UpdateCallPurpose(request);
            return Ok(new { message = "Call Purpose Name created successfully." });
        }

        [HttpPost("get-all-callpurpose-list")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetAllCallPurposeList(ExcludedKeywordsListRequest param)
        {
            CallPurposeResponseModel<CallPurpose> list = await _callPurposeRepository.GetAllCallPurposeList(param);
            return Ok(list);
        }

        [HttpGet("get-callpurpose-id/{guid}")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetCallPurposeById(Guid guid)
        {
            CallPurpose? list = await _callPurposeRepository.GetCallPurposeById(guid);
            return Ok(list);
        }

        [HttpGet("get-callpurpose-retrieveall")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetCallPurposeRetrieveAll()
        {
            IEnumerable<CallPurpose>? list = await _callPurposeRepository.GetCallPurposeRetrievedAll();
            return Ok(list);
        }

        [HttpGet("delete-callpurpose/{guid}")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> DeleteCallPurpose(Guid guid)
        {
            await _callPurposeRepository.DeleteCallPurpose(guid);
            return Ok(new { message = "Call purpose deleted successfully." });
        }
    }
}
