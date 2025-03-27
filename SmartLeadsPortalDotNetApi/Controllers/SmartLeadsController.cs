using Microsoft.AspNetCore.Mvc;
using SmartLeadsPortalDotNetApi.Services;
using SmartLeadsPortalDotNetApi.Services.Model;

namespace SmartLeadsPortalDotNetApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SmartLeadsController : ControllerBase
    {
        private readonly SmartLeadsApiService _smartLeadsApiService;
        public SmartLeadsController(SmartLeadsApiService smartLeadsApiService)
        {
            _smartLeadsApiService = smartLeadsApiService;
        }

        [HttpGet("get-campaigns")]
        public async Task<IActionResult> GetSmartLeadsCampaigns()
        {
            try
            {
                var campaigns = await _smartLeadsApiService.GetSmartLeadsCampaigns();
                return Ok(campaigns ?? new List<SmartLeadsCampaign>());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("get-campaign-by-id/{id}")]
        public async Task<IActionResult> GetSmartLeadsCampaignById(int id)
        {
            try
            {
                var campaigns = await _smartLeadsApiService.GetSmartLeadsCampaignById(id);
                return Ok(campaigns ?? new SmartLeadsCampaign());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("get-leads-by-campaignid/{id}")]
        public async Task<IActionResult> GetLeadsByCampaignId(int id)
        {
            try
            {
                var leads = await _smartLeadsApiService.GetLeadsByCampaignId(id);
                return Ok(leads ?? new SmartLeadsResponse());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("get-leads-all-account")]
        public async Task<IActionResult> GetAllLeadsAllAccount([FromQuery] string? createdDate = null, string? email = null, int offset = 0, int limit = 10)
        {
            try
            {
                var leads = await _smartLeadsApiService.GetAllLeadsAllAccount(createdDate, email, offset, limit);
                return Ok(leads ?? new SmartLeadsAllLeadsResponse());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("get-sequence-analytics-campaign")]
        public async Task<IActionResult> GetSequenceAnaylyticByCampaign([FromQuery] int id, string? start_date = null, string? end_date = null, string? time_zone = null)
        {
            try
            {
                var leads = await _smartLeadsApiService.GetSequenceAnaylyticByCampaign(id, start_date, end_date, time_zone);
                return Ok(leads ?? new CampaignAnalyticsResponse());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("get-analytics-campaign-daterange")]
        public async Task<IActionResult> GetAnalyticsByCampaignDateRange([FromQuery] int id, string? start_date = null, string? end_date = null)
        {
            try
            {
                var leads = await _smartLeadsApiService.GetAnalyticsByCampaignDateRange(id, start_date, end_date);
                return Ok(leads ?? new CampaignAnalyticsDateRange());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
