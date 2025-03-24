using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Repositories;

namespace SmartLeadsPortalDotNetApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExcludedKeywordsController : ControllerBase
    {
        private readonly IExcludedKeywordsRepository _excludedKeywordsRepository;
        private readonly HttpClient _httpClient;

        public ExcludedKeywordsController(IExcludedKeywordsRepository excludedKeywordsRepository, HttpClient httpClient)
        {
            _excludedKeywordsRepository = excludedKeywordsRepository;
            _httpClient = httpClient;
        }

        // GET: api/ExcludedKeywords
        [HttpPost("getAllKeywords")]
        public async Task<IActionResult> GetAllKeywords([FromBody] ExcludedKeywordsListRequest request)
        {
            try
            {
                var result = await _excludedKeywordsRepository.GetAllKeywords(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("getAllKeywordsMap")]
        public async Task<IActionResult> GetAllKeywordsMap()
        {
            try
            {
                var result = await _excludedKeywordsRepository.GetAllKeywordsMap();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("getKeywordById/{id}")]
        public async Task<IActionResult> GetKeywordById(int id)
        {
            try
            {
                var result = await _excludedKeywordsRepository.GetKeywordById(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("insertKeywords")]
        public async Task<IActionResult> InsertKeyword([FromBody] ExcludedKeywordsInsert request)
        {
            if (string.IsNullOrEmpty(request.ExludedKeywords))
            {
                return BadRequest(new { error = "Keyword text is required." });
            }

            await _excludedKeywordsRepository.InsertKeyword(request);
            return Ok(new { message = "keywords created successfully." });
        }

        [HttpPost("updateKeyword")]
        public async Task<IActionResult> UpdateKeyword([FromBody] ExcludedKeywords request)
        {
            if (request.Id == 0)
            {
                return BadRequest(new { error = "ID is required to update keyword." });
            }

            await _excludedKeywordsRepository.UpdateKeyword(request);
            return Ok(new { message = "keywords updated successfully." });
        }

        [HttpPost("deleteKeyword")]
        public async Task<IActionResult> DeleteKeyword([FromBody] ExcludedKeywordsUpdateRequest request)
        {
            if (request.Id == 0)
            {
                return BadRequest(new { error = "ID is required to delete keyword." });
            }

            await _excludedKeywordsRepository.DeleteKeyword(request.Id);
            return Ok(new { message = "keywords deleted successfully." });
        }
    }
}
