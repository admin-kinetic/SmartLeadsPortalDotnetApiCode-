using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Repositories;
using SmartLeadsPortalDotNetApi.Services;

namespace SmartLeadsPortalDotNetApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CallDispositionController : ControllerBase
    {
        private readonly CallDispositionRepository _callDispositionRepository;
        public CallDispositionController(CallDispositionRepository callDispositionRepository)
        {
            _callDispositionRepository = callDispositionRepository;
        }

        [HttpPost("insert-call-disposition")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> InsertCallDisposition([FromBody] CallDispositionInsert request)
        {
            if (string.IsNullOrEmpty(request.CallDispositionName))
            {
                return BadRequest(new { error = "Call Disposition Name text is required." });
            }

            await _callDispositionRepository.InsertCallDisposition(request);
            return Ok(new { message = "Call Disposition Name created successfully." });
        }

        [HttpPost("update-call-disposition")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> UpdateCallDisposition([FromBody] CallDisposition request)
        {
            if (string.IsNullOrEmpty(request.CallDispositionName))
            {
                return BadRequest(new { error = "Call Disposition Name text is required." });
            }

            await _callDispositionRepository.UpdateCallDisposition(request);
            return Ok(new { message = "Call Disposition Name created successfully." });
        }

        [HttpPost("get-all-calldisposition-list")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetAllCallDispositionList(ExcludedKeywordsListRequest param)
        {
            CallDispositionResponseModel<CallDisposition> list = await _callDispositionRepository.GetAllCallDispositionList(param);
            return Ok(list);
        }

        [HttpGet("get-calldisposition-id/{guid}")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetCallDispositionById(Guid guid)
        {
            CallDisposition? list = await _callDispositionRepository.GetCallDispositionById(guid);
            return Ok(list);
        }

        [HttpGet("get-calldisposition-retrieveall")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetCallDispositionRetrieveAll()
        {
            IEnumerable<CallDisposition>? list = await _callDispositionRepository.GetCallDispositionRetrievedAll();
            return Ok(list);
        }

        [HttpGet("delete-calldisposition/{guid}")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> DeleteCallDisposition(Guid guid)
        {
            await _callDispositionRepository.DeleteCallDisposition(guid);
            return Ok(new { message = "Call Disposition deleted successfully." });
        }
    }
}
