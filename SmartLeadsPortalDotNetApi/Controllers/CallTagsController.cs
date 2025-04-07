using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Repositories;

namespace SmartLeadsPortalDotNetApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CallTagsController : ControllerBase
    {
        private readonly CallTagRepository _callTagRepository;
        public CallTagsController(CallTagRepository callTagRepository)
        {
            _callTagRepository = callTagRepository;
        }

        [HttpPost("insert-call-tags")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> InsertCallTags([FromBody] CallTagsInsert request)
        {
            if (string.IsNullOrEmpty(request.TagName))
            {
                return BadRequest(new { error = "Tag Name text is required." });
            }

            await _callTagRepository.InsertCallTags(request);
            return Ok(new { message = "Tag Name created successfully." });
        }

        [HttpPost("update-call-tags")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> UpdateCallTags([FromBody] CallTags request)
        {
            if (string.IsNullOrEmpty(request.TagName))
            {
                return BadRequest(new { error = "Call Tag Name text is required." });
            }

            await _callTagRepository.UpdateCallTags(request);
            return Ok(new { message = "Call Tag Name created successfully." });
        }

        [HttpPost("get-all-calltag-list")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetAllCallTagsList(ExcludedKeywordsListRequest param)
        {
            CallTagsResponseModel<CallTags> list = await _callTagRepository.GetAllCallTagsList(param);
            return Ok(list);
        }

        [HttpGet("get-calltags-id/{guid}")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetCallTagsById(Guid guid)
        {
            CallTags? list = await _callTagRepository.GetCallTagsById(guid);
            return Ok(list);
        }

        [HttpGet("get-calltags-retrieveall")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetCallTagsRetrieveAll()
        {
            IEnumerable<CallTags>? list = await _callTagRepository.GetCallTagsRetrievedAll();
            return Ok(list);
        }

        [HttpGet("delete-calltag/{guid}")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> DeleteCallTags(Guid guid)
        {
            await _callTagRepository.DeleteCallTags(guid);
            return Ok(new { message = "Call Tags deleted successfully." });
        }
    }
}
