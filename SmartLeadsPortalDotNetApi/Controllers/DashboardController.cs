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
    public class DashboardController : ControllerBase
    {
        private readonly DashboardRepository _dashboardRepository;
        public DashboardController(DashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        [HttpGet("get-total-urgent-task")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetDashboardUrgentTaskTotal(CancellationToken cancellationToken)
        {
            var result = await _dashboardRepository.GetDashboardUrgentTaskTotal(cancellationToken);
            return Ok(result);
        }

        [HttpGet("get-total-high-task")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetDashboardHighTaskTotal(CancellationToken cancellationToken)
        {
            var result = await _dashboardRepository.GetDashboardHighTaskTotal(cancellationToken);
            return Ok(result);
        }

        [HttpGet("get-total-low-task")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetDashboardLowTaskTotal(CancellationToken cancellationToken)
        {
            var result = await _dashboardRepository.GetDashboardLowTaskTotal(cancellationToken);
            return Ok(result);
        }

        [HttpGet("get-total-due-task")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetDashboardPastDueTaskTotal(CancellationToken cancellationToken)
        {
            var result = await _dashboardRepository.GetDashboardPastDueTaskTotal(cancellationToken);
            return Ok(result);
        }

        [HttpGet("get-total-prospect")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetDashboardProspectTotal(CancellationToken cancellationToken)
        {
            var result = await _dashboardRepository.GetDashboardProspectTotal(cancellationToken);
            return Ok(result);
        }

        [HttpGet("get-todo-taskdue")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetDashboardTodoTaskDue(CancellationToken cancellationToken)
        {
            var result = await _dashboardRepository.GetDashboardTodoTaskDue(cancellationToken);
            return Ok(result);
        }

        [HttpGet("get-urgent-task-list")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetDashboardUrgentTaskList(CancellationToken cancellationToken)
        {
            var result = await _dashboardRepository.GetDashboardUrgentTaskList(cancellationToken);
            return Ok(result);
        }

        [HttpPost("get-dashboard-emailcampaign-bdr")]
        public async Task<IActionResult> GetDashboardEmailCampaignBDR(DashboardDateParameter param)
        {
            var ret = await this._dashboardRepository.GetDashboardEmailCampaignBDR(param);
            return Ok(ret);
        }

        [HttpPost("get-dashboard-emailcampaign-createdby")]
        public async Task<IActionResult> GetDashboardEmailCampaignCreatedBy(DashboardDateParameter param)
        {
            var ret = await this._dashboardRepository.GetDashboardEmailCampaignCreatedBy(param);
            return Ok(ret);
        }

        [HttpPost("get-dashboard-emailcampaign-qaby")]
        public async Task<IActionResult> GetDashboardEmailCampaignQaBy(DashboardDateParameter param)
        {
            var ret = await this._dashboardRepository.GetDashboardEmailCampaignQaBy(param);
            return Ok(ret);
        }

        [HttpPost("get-dashboard-job-adchart-emailcampaign")]
        public async Task<IActionResult> GetDashboardJobAdChartsEmailSequenceLeadgen(DashboardDateParameter param)
        {
            var ret = await this._dashboardRepository.GetDashboardJobAdChartsEmailSequenceLeadgen(param);
            return Ok(ret);
        }

        [HttpPost("get-dashboard-job-adchart-emailsequence")]
        public async Task<IActionResult> GetDashboardJobAdChartsEmailSequenceFullyAutomated(DashboardDateParameter param)
        {
            var ret = await this._dashboardRepository.GetDashboardJobAdChartsEmailSequenceFullyAutomated(param);
            return Ok(ret);
        }

        [HttpPost("get-dashboard-job-adchart-exported-leadgen")]
        public async Task<IActionResult> GetDashboardJobAdChartsExportedLeadgen(DashboardDateParameter param)
        {
            var ret = await this._dashboardRepository.GetDashboardJobAdChartsExportedLeadgen(param);
            return Ok(ret);
        }

        [HttpPost("get-dashboard-job-adchart-exported")]
        public async Task<IActionResult> GetDashboardJobAdChartsExportedFullyAutomated(DashboardDateParameter param)
        {
            var ret = await this._dashboardRepository.GetDashboardJobAdChartsExportedFullyAutomated(param);
            return Ok(ret);
        }
    }
}
