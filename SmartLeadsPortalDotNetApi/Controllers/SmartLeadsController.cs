using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SmartLeadsPortalDotNetApi.Entities;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Repositories;
using SmartLeadsPortalDotNetApi.Services;
using SmartLeadsPortalDotNetApi.Services.Model;

namespace SmartLeadsPortalDotNetApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SmartLeadsController : ControllerBase
    {
        private readonly SmartLeadsApiService _smartLeadsApiService;
        private readonly SmartLeadsRepository _smartLeadsRepository;
        private readonly CallTasksTableRepository callTasksTableRepository;
        private readonly CallsTableRepository callsTableRepository;

        public SmartLeadsController(SmartLeadsApiService smartLeadsApiService, SmartLeadsRepository smartLeadsRepository, CallTasksTableRepository callTasksTableRepository, CallsTableRepository callsTableRepository)
        {
            _smartLeadsApiService = smartLeadsApiService;
            _smartLeadsRepository = smartLeadsRepository;
            this.callTasksTableRepository = callTasksTableRepository;
            this.callsTableRepository = callsTableRepository;
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
        public async Task<IActionResult> GetLeadsByCampaignId(int id, [FromQuery] int offset, int limit)
        {
            try
            {
                var leads = await _smartLeadsApiService.GetLeadsByCampaignId(id, offset, limit);
                return Ok(leads ?? new SmartLeadsResponse());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("get-leads-all-account")]
        public async Task<IActionResult> GetAllLeadsAllAccount([FromQuery] string? createdDate = null, string? email = null, int offset = 0, int limit = 0)
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

        [HttpGet("get-statistics-campaign")]
        public async Task<IActionResult> GetStatisticsByCampaign([FromQuery] int id, int offset = 0, int limit = 10)
        {
            try
            {
                var leads = await _smartLeadsApiService.GetStatisticsByCampaign(id, offset, limit);
                return Ok(leads ?? new CampaignStatisticsResponse());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("get-analytics-campaign")]
        public async Task<IActionResult> GetAnalyticsByCampaign([FromQuery] int id)
        {
            try
            {
                var leads = await _smartLeadsApiService.GetAnalyticsByCampaign(id);
                return Ok(leads ?? new CampaignAnalytics());
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

        [HttpPost("get-all-smartleads-calltasks")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetAllSmartLeadsCallTaskList(SmartLeadsCallTasksRequest param)
        {
            SmartLeadsCallTasksResponseModel<SmartLeadsCallTasks> list = await _smartLeadsRepository.GetAllSmartLeadsCallTaskList(param);
            return Ok(list);
        }

        [HttpPost("call-tasks/find")]
        public async Task<IActionResult> CallTasksFind(TableRequest request){
            var result = await this.callTasksTableRepository.Find(request);
            return Ok(result);
        }

        [HttpGet("call-tasks/columns/all")]
        public IActionResult CallTasksColumnsAll(){
            var result = this.callTasksTableRepository.AllColumns();
            result.Sort();
            return Ok(new { data = result });
        }

        [HttpPost("calls/find")]
        public async Task<IActionResult> CallsFind(TableRequest request)
        {
            var result = await this.callsTableRepository.Find(request);
            //var result = new TableResponse<SmartleadsCalls>
            //{
            //    Items = new List<SmartleadsCalls>{
            //        new SmartleadsCalls{
            //            Id = 1,
            //            User = "Ralph Malaga",
            //            ProspectName = "Jane Smith",
            //            ProspectPhoneNumber = "123-456-7890",
            //            DateTime = "2023-10-01T12:00:00Z",
            //            State = "Completed",
            //            Duration = "00:30:00",
            //            Purpose = "Follow-up",
            //            Disposition = "Interested",
            //            Notes = "Discussed product features and pricing."
            //        }
            //    },
            //    Total = 1

            //};
            return Ok(result);
        }

        [HttpGet("calls/columns/all")]
        public IActionResult CallsColumnsAll()
        {
            var result = this.callsTableRepository.AllColumns();
            result.Sort();
            //var result = new List<string>{
            //    "Id",
            //    "User",
            //    "ProspectName",
            //    "ProspectPhoneNumber",
            //    "DateTime",
            //    "State",
            //    "Duration",
            //    "Purpose",
            //    "Disposition",
            //    "Notes"
            //};
            return Ok(new { data = result });
        }

        [HttpPost("update-call-tasks")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> UpdateCallTasks([FromBody] CallTasksUpdateParam request)
        {
            var ret = await this.callTasksTableRepository.UpdateCallTasks(request);
            return Ok(ret);
        }
    }
}
