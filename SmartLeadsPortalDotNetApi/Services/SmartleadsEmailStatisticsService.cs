using System;
using SmartLeadsPortalDotNetApi.Helper;
using SmartLeadsPortalDotNetApi.Model.Webhooks.Emails;
using SmartLeadsPortalDotNetApi.Repositories;

namespace SmartLeadsPortalDotNetApi.Services;

public class SmartleadsEmailStatisticsService
{
    private readonly ILogger<SmartleadsEmailStatisticsService> _logger;
    private readonly SmartLeadsEmailStatisticsRepository _smartLeadsEmailStatisticsRepository;
    private readonly SmartLeadsAllLeadsRepository _smartLeadsAllLeadsRepository;
    private readonly SmartLeadsApiService _smartLeadsApiService;
    private readonly SmartleadCampaignRepository smartleadCampaignRepository;

    public SmartleadsEmailStatisticsService(
        ILogger<SmartleadsEmailStatisticsService> _logger,
        SmartLeadsEmailStatisticsRepository smartLeadsEmailStatisticsRepository,
        SmartLeadsAllLeadsRepository smartLeadsAllLeadsRepository,
        SmartLeadsApiService smartLeadsApiService,
        SmartleadCampaignRepository smartleadCampaignRepository)
    {
        this._logger = _logger;
        _smartLeadsEmailStatisticsRepository = smartLeadsEmailStatisticsRepository;
        _smartLeadsAllLeadsRepository = smartLeadsAllLeadsRepository;
        _smartLeadsApiService = smartLeadsApiService;
        this.smartleadCampaignRepository = smartleadCampaignRepository;
    }

    public async Task UpdateEmailReply(EmailReplyPayload payloadObject)
    {
        var lead = await _smartLeadsAllLeadsRepository.GetByEmail(payloadObject.to_email);
        if (lead == null)
        {
            var campaignId = payloadObject.campaign_id;
            var account = await this.smartleadCampaignRepository.GetAccountByCampaignId(campaignId);

            _logger.LogInformation("Email not found in leads. Try to retrieve from SmartLeads.");
            var leadFromSmartLeads = await RetryHelper.ExecuteWithRetryAsync(async () =>
            {
                return await _smartLeadsApiService.GetLeadByEmail(payloadObject.to_email, account.Id);
            });

            if (leadFromSmartLeads == null)
            {
                throw new ArgumentException("Email not found in both in our database or smartleads.");
            }

            await _smartLeadsAllLeadsRepository.InsertLeadFromSmartleads(leadFromSmartLeads);
        }

        await _smartLeadsEmailStatisticsRepository.UpdateEmailReply(payloadObject);
    }

    public async Task UpdateEmailSent(EmailSentPayload payloadObject)
    {
        var lead = await _smartLeadsAllLeadsRepository.GetByEmail(payloadObject.to_email);
        if (lead == null)
        {
            var campaignId = payloadObject.campaign_id;
            var account = await this.smartleadCampaignRepository.GetAccountByCampaignId(campaignId);

            if (account == null)
            {
                throw new ArgumentException($"Account not found for {campaignId} campaign in both in our database or smartleads.");
            }

            _logger.LogInformation("Email not found in leads. Try to retrieve from SmartLeads.");
            var leadFromSmartLeads = await RetryHelper.ExecuteWithRetryAsync(async () =>
            {
                return await _smartLeadsApiService.GetLeadByEmail(payloadObject.to_email, account.Id);
            });

            if (leadFromSmartLeads == null)
            {
                throw new ArgumentException("Email not found in both in our database or smartleads.");
            }

            await _smartLeadsAllLeadsRepository.InsertLeadFromSmartleads(leadFromSmartLeads);
        }

        await _smartLeadsEmailStatisticsRepository.UpsertEmailSent(payloadObject);
    }
}
