﻿using Microsoft.AspNetCore.Authorization;
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
    public class DashboardController : ControllerBase
    {
        private readonly DashboardRepository _dashboardRepository;
        private readonly SmartLeadsApiService _smartLeadsApiService;
        public DashboardController(DashboardRepository dashboardRepository, SmartLeadsApiService smartLeadsApiService)
        {
            _dashboardRepository = dashboardRepository;
            _smartLeadsApiService = smartLeadsApiService;
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

        [HttpPost("get-dashboard-emailcampaign-bdr-for-csv")]
        public async Task<IActionResult> GetDashboardEmailCampaignBdrForCsv(DashboardDateParameter param)
        {
            var ret = await this._dashboardRepository.GetDashboardEmailCampaignBdrForCsv(param);
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

        [HttpPost("get-dashboard-analytics-daterange")]
        public async Task<IActionResult> GetDashboardAnalyticsByDates(DashboardDateParameter param)
        {
            string startDateStr = param.StartDate?.ToString("yyyy-MM-dd") ?? string.Empty;
            string endDateStr = param.EndDate?.ToString("yyyy-MM-dd") ?? string.Empty;
            var campaignList = await _dashboardRepository.GetDashboardSmartLeadCampaignsActive();

            int totalLeadsReached = 0;
            int totalSent = 0;
            int totalOpened = 0;
            int totalUniqueOpened = 0;
            int totalUniqueReplied = 0;
            int totalBounced = 0;
            int totalPositiveReplies = 0;

            foreach (var campaign in campaignList)
            {
                var analytics = await _smartLeadsApiService.GetAnalyticsByCampaignDateRange(campaign.Id, startDateStr, endDateStr);
                var positiveAnalytics = await _smartLeadsApiService.GetAnalyticsByCampaign(campaign.Id);

                // Convert string values to integers
                totalLeadsReached += int.TryParse(analytics?.sent_count, out var t1) ? t1 : 0;
                totalSent += int.TryParse(analytics?.unique_sent_count, out var t2) ? t2 : 0;
                totalOpened += int.TryParse(analytics?.open_count, out var t3) ? t3 : 0;
                totalUniqueOpened += int.TryParse(analytics?.unique_open_count, out var t4) ? t4 : 0;
                totalUniqueReplied += int.TryParse(analytics?.reply_count, out var t5) ? t5 : 0;
                totalBounced += int.TryParse(analytics?.bounce_count, out var t6) ? t6 : 0;
                totalPositiveReplies += positiveAnalytics?.campaign_lead_stats?.interested ?? 0;
            }

            double positiveRepliesRate = (totalLeadsReached > 0) ? ((double)totalPositiveReplies / totalLeadsReached) * 100 : 0;
            double uniqueRepliesRate = (totalLeadsReached > 0) ? ((double)totalUniqueReplied / totalLeadsReached) * 100 : 0;
            double uniqueOpenRate = (totalLeadsReached > 0) ? ((double)totalUniqueOpened / totalLeadsReached) * 100 : 0;
            double uniqueBounceRate = (totalLeadsReached > 0) ? ((double)totalBounced / totalLeadsReached) * 100 : 0;

            var totalEmailSent = totalSent + totalBounced;

            // Return single aggregated result
            var result = new
            {
                TotalLeadsReached = totalLeadsReached,
                TotalSent = totalSent,
                TotalOpened = totalOpened,
                TotalUniqueOpened = totalUniqueOpened,
                TotalUniqueReplied = totalUniqueReplied,
                TotalBounced = totalBounced,
                TotalPositiveReplies = totalPositiveReplies,
                PositiveRepliesRate = Math.Round(positiveRepliesRate, 2),
                UniqueRepliesRate = Math.Round(uniqueRepliesRate, 2),
                UniqueOpenRate = Math.Round(uniqueOpenRate, 2),
                UniqueBounceRate = Math.Round(uniqueBounceRate, 2),
                TotalEmailSent = totalEmailSent,
                TotalEmailed = totalEmailSent + totalBounced
            };

            return Ok(result);
        }

        [HttpPost("get-dashboard-email-statistics-totalsent")]
        public async Task<IActionResult> GetDashboardEmailStatisticsTotalSent(DashboardDateParameter param)
        {
            var ret = await this._dashboardRepository.GetDashboardEmailStatisticsTotalSent(param);
            return Ok(ret);
        }

        [HttpGet("get-bdr-list")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetDashboardBDRList(CancellationToken cancellationToken)
        {
            var result = await _dashboardRepository.GetDashboardBDRList(cancellationToken);
            return Ok(result);
        }

        [HttpGet("get-campaign-list")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetDashboardCampaignsList(CancellationToken cancellationToken)
        {
            var result = await _dashboardRepository.GetDashboardCampaignsList(cancellationToken);
            return Ok(result);
        }

        [HttpGet("get-leadgen-list")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetDashboardLeadgenList(CancellationToken cancellationToken)
        {
            var result = await _dashboardRepository.GetDashboardLeadgenList(cancellationToken);
            return Ok(result);
        }

        [HttpGet("get-qa-list")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetDashboardQaList(CancellationToken cancellationToken)
        {
            var result = await _dashboardRepository.GetDashboardQaList(cancellationToken);
            return Ok(result);
        }

        //Smartleads Analytics Dashboard
        [HttpPost("get-dashboard-smartleads-analytics-filter")]
        public async Task<IActionResult> GetDashboardAnalyticsFilter(DashboardFilterModel param)
        {
            string startDateStr = param.StartDate.ToString("yyyy-MM-dd") ?? string.Empty;
            string endDateStr = param.EndDate.ToString("yyyy-MM-dd") ?? string.Empty;

            int? totalLeadsReached = 0;
            int? totalSent = 0;
            int? totalOpened = 0;
            int? totalUniqueOpened = 0;
            int? totalUniqueReplied = 0;
            int? totalBounced = 0;
            int? totalPositiveReplies = 0;
            int? totalExported = 0;
            int? totalEmailed = 0;
            int? totalOutOfOffice = 0;
            int? totalIncorrect = 0;

            param.StartDate = DateTime.Parse(startDateStr);
            param.EndDate = DateTime.Parse(endDateStr);

            totalSent = (await _dashboardRepository.GetDashboardSmartLeadAnalyticsTotalSent(param)).TotalSent;
            totalOpened = (await _dashboardRepository.GetDashboardSmartLeadAnalyticsTotalOpened(param)).TotalOpened;
            totalUniqueOpened = (await _dashboardRepository.GetDashboardSmartLeadAnalyticsTotalUniqueOpened(param)).TotalUniqueOpened;
            totalUniqueReplied = (await _dashboardRepository.GetDashboardSmartLeadAnalyticsTotalReply(param)).TotalReplied;
            totalBounced = (await _dashboardRepository.GetDashboardSmartLeadAnalyticsTotalBounced(param)).TotalBounced;
            totalPositiveReplies = (await _dashboardRepository.GetDashboardSmartLeadAnalyticsTotalInterested(param)).TotalInterested;
            totalExported = (await _dashboardRepository.GetDashboardSmartLeadAnalyticsTotalExported(param)).TotalExported;
            totalOutOfOffice = (await _dashboardRepository.GetDashboardSmartLeadAnalyticsTotalOutOfOffice(param)).TotalOutOfOffice;
            totalIncorrect = (await _dashboardRepository.GetDashboardSmartLeadAnalyticsTotalIncorrectContact(param)).TotalIncorrectContact;

            totalLeadsReached = totalSent - totalBounced;
            totalEmailed = totalSent + totalBounced;

            double? positiveRepliesRate = (totalLeadsReached > 0) ? ((double?)totalPositiveReplies / totalLeadsReached) * 100 : 0;
            double? uniqueRepliesRate = (totalLeadsReached > 0) ? ((double?)totalUniqueReplied / totalLeadsReached) * 100 : 0;
            double? uniqueOpenRate = (totalLeadsReached > 0) ? ((double?)totalUniqueOpened / totalLeadsReached) * 100 : 0;
            double? uniqueBounceRate = (totalLeadsReached > 0) ? ((double?)totalBounced / totalLeadsReached) * 100 : 0;

            // Ensure uniqueBounceRate is not null before casting to decimal
            decimal uniqueBounceRateValue = (decimal)(uniqueBounceRate ?? 0);

            // Return single aggregated result
            var result = new
            {
                TotalLeadsReached = totalLeadsReached,
                TotalSent = totalSent,
                TotalOpened = totalOpened,
                TotalUniqueOpened = totalUniqueOpened,
                TotalUniqueReplied = totalUniqueReplied,
                TotalBounced = totalBounced,
                TotalPositiveReplies = totalPositiveReplies,
                PositiveRepliesRate = Math.Round(positiveRepliesRate ?? 0, 2),
                UniqueRepliesRate = Math.Round(uniqueRepliesRate ?? 0, 2),
                UniqueOpenRate = Math.Round(uniqueOpenRate ?? 0, 2),
                UniqueBounceRate = Math.Round(uniqueBounceRateValue, 2),
                TotalExported = totalExported,
                TotalEmailed = totalEmailed,
                TotalOutOfOffice = totalOutOfOffice,
                TotalIncorrect = totalIncorrect
            };

            return Ok(result);
        }

        [HttpPost("get-dashboard-emailcampaign-bdr-chart")]
        public async Task<IActionResult> GetDashboardEmailCampaignBDRChart(DashboardFilterModel param)
        {
            var ret = await this._dashboardRepository.GetDashboardEmailCampaignBDRChart(param);
            return Ok(ret);
        }

        [HttpPost("get-dashboard-emailcampaign-createdby-chart")]
        public async Task<IActionResult> GetDashboardEmailCampaignCreatedByChart(DashboardFilterModel param)
        {
            var ret = await this._dashboardRepository.GetDashboardEmailCampaignCreatedByChart(param);
            return Ok(ret);
        }

        [HttpPost("get-dashboard-emailcampaign-qaby-chart")]
        public async Task<IActionResult> GetDashboardEmailCampaignQaByChart(DashboardFilterModel param)
        {
            var ret = await this._dashboardRepository.GetDashboardEmailCampaignQaByChart(param);
            return Ok(ret);
        }

        [HttpPost("get-dashboard-leads-exported-charts")]
        public async Task<IActionResult> GetDashboardAutomatedLeadsCampaignExportedCharts(DashboardFilterModel param)
        {
            var ret = await this._dashboardRepository.GetDashboardAutomatedLeadsCampaignExportedCharts(param);
            return Ok(ret);
        }

        [HttpPost("get-dashboard-leads-emailsequence-charts")]
        public async Task<IActionResult> GetDashboardAutomatedLeadsCampaignEmailSequenceCharts(DashboardFilterModel param)
        {
            var ret = await this._dashboardRepository.GetDashboardAutomatedLeadsCampaignEmailSequenceCharts(param);
            return Ok(ret);
        }

        [HttpPost("get-dashboard-bdr-charts-csv")]
        public async Task<IActionResult> GetDashboardAutomatedBdrChartsCsvDownload(DashboardFilterModel param)
        {
            var ret = await this._dashboardRepository.GetDashboardAutomatedBdrChartsCsvDownload(param);
            return Ok(ret);
        }

        [HttpPost("get-dashboard-leadsexported-charts-csv")]
        public async Task<IActionResult> GetDashboardLeadsExportedCsvDownload(DashboardFilterModel param)
        {
            var ret = await this._dashboardRepository.GetDashboardLeadsExportedCsvDownload(param);
            return Ok(ret);
        }

        [HttpPost("get-dashboard-emailsequence-charts-csv")]
        public async Task<IActionResult> GetDashboardEmailSequenceCsvDownload(DashboardFilterModel param)
        {
            var ret = await this._dashboardRepository.GetDashboardEmailSequenceCsvDownload(param);
            return Ok(ret);
        }
    }
}
