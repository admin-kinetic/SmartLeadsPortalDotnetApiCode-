using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Repositories;

namespace SmartLeadsPortalDotNetApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ExcludedKeywordsController : ControllerBase
    {
        private readonly ExcludedKeywordsRepository _excludedKeywordsRepository;
        private readonly HttpClient _httpClient;

        public ExcludedKeywordsController(ExcludedKeywordsRepository excludedKeywordsRepository, HttpClient httpClient)
        {
            _excludedKeywordsRepository = excludedKeywordsRepository;
            _httpClient = httpClient;
        }

        //MSSQL API
        [HttpPost("insertExcludedKeywords")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> InsertExcludedKeyword([FromBody] ExcludedKeywordsInsert request)
        {
            if (string.IsNullOrEmpty(request.ExludedKeywords))
            {
                return BadRequest(new { error = "Keyword text is required." });
            }

            await _excludedKeywordsRepository.InsertExcludedKeyword(request);
            return Ok(new { message = "keywords created successfully." });
        }

        [HttpPost("updateExcludedKeywords")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> UpdateExcludedKeyword([FromBody] ExcludedKeywordsModel request)
        {
            if (string.IsNullOrEmpty(request.ExcludedKeyword))
            {
                return BadRequest(new { error = "Keyword text is required." });
            }

            await _excludedKeywordsRepository.UpdateExcludedKeyword(request);
            return Ok(new { message = "keywords created successfully." });
        }

        [HttpPost("getAllKeywordsList")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetAllExcludeKeywordsList(ExcludedKeywordsListRequest param)
        {
            ExcludedKeywordsResponseModel<ExcludedKeywordsModel> list = await _excludedKeywordsRepository.GetAllExcludeKeywordsList(param);
            return Ok(list);
        }

        [HttpGet("getexcludedkeywordsbyid/{guid}")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetExcludeKeywordsById(Guid guid)
        {
            ExcludedKeywordsModel? list = await _excludedKeywordsRepository.GetExcludeKeywordsById(guid);
            return Ok(list);
        }

        [HttpGet("getallexcludedkeywordsmap")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetAllExcludeKeywordsMap()
        {
            IEnumerable<ExcludedKeywordsModel> list = await _excludedKeywordsRepository.GetAllExcludeKeywordsMap();
            return Ok(list);
        }

        [HttpGet("deleteexcludedkeywrods/{guid}")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> DeleteExcludedKeyword(Guid guid)
        {
            await _excludedKeywordsRepository.DeleteExcludedKeyword(guid);
            return Ok(new { message = "keywords deleted successfully." });
        }

        //MYSQL API

        // GET: api/ExcludedKeywords
        [HttpPost("getAllKeywords")]
        [EnableCors("CorsApi")]
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
        [EnableCors("CorsApi")]
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
        [EnableCors("CorsApi")]
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
        [EnableCors("CorsApi")]
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
        [EnableCors("CorsApi")]
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
        [EnableCors("CorsApi")]
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
