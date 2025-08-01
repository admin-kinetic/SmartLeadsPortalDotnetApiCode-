using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Data.SqlClient;
using Polly;
using SmartLeadsPortalDotNetApi.Database;
using SmartLeadsPortalDotNetApi.Entities;
using SmartLeadsPortalDotNetApi.Helper;
using SmartLeadsPortalDotNetApi.Model.Webhooks.Emails;
using SmartLeadsPortalDotNetApi.Repositories;
using SmartLeadsPortalDotNetApi.Services.Model;

namespace SmartLeadsPortalDotNetApi.Services;

public class WebhookService
{
    private readonly AutomatedLeadsRepository automatedLeadsRepository;
    private readonly WebhooksRepository webhooksRepository;
    private readonly LeadClicksRepository leadClicksRepository;
    private readonly SmartLeadsEmailStatisticsRepository _smartLeadsEmailStatisticsRepository;
    private readonly MessageHistoryRepository _messageHistoryRepository;
    private readonly SmartLeadsAllLeadsRepository smartLeadsAllLeadsRepository;
    private readonly DbExecution dbExecution;
    private readonly SmartleadsEmailStatisticsService _smartleadsEmailStatisticsService;
    private readonly SmartLeadsApiService smartLeadsApiService;
    private readonly SmartleadCampaignRepository smartleadCampaignRepository;
    private readonly SmartleadAccountRepository smartleadAccountRepository;
    private readonly ILogger<WebhookService> logger;
    private readonly IAsyncPolicy _retryPolicy;

    public WebhookService(
        AutomatedLeadsRepository automatedLeadsRepository,
        WebhooksRepository webhooksRepository,
        LeadClicksRepository leadClicksRepository,
        SmartLeadsExportedContactsRepository smartLeadsExportedContactsRepository,
        SmartLeadsEmailStatisticsRepository smartLeadsEmailStatisticsRepository,
        MessageHistoryRepository messageHistoryRepository,
        SmartLeadsAllLeadsRepository smartLeadsAllLeadsRepository,
        DbExecution dbExecution,
        SmartleadsEmailStatisticsService smartleadsEmailStatisticsService,
        SmartLeadsApiService smartLeadsApiService,
        SmartleadCampaignRepository smartleadCampaignRepository,
        SmartleadAccountRepository smartleadAccountRepository,
        ILogger<WebhookService> logger)
    {
        this.automatedLeadsRepository = automatedLeadsRepository;
        this.webhooksRepository = webhooksRepository;
        this.leadClicksRepository = leadClicksRepository;
        _smartLeadsEmailStatisticsRepository = smartLeadsEmailStatisticsRepository;
        _messageHistoryRepository = messageHistoryRepository;
        this.smartLeadsAllLeadsRepository = smartLeadsAllLeadsRepository;
        this.dbExecution = dbExecution;
        _smartleadsEmailStatisticsService = smartleadsEmailStatisticsService;
        this.smartLeadsApiService = smartLeadsApiService;
        this.smartleadCampaignRepository = smartleadCampaignRepository;
        this.smartleadAccountRepository = smartleadAccountRepository;
        this.logger = logger;
        
        _retryPolicy = Policy
            .Handle<SqlException>()
            .WaitAndRetryAsync(3,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, timeSpan, retryCount, context) => {
                    this.logger.LogWarning(
                        "Retry {RetryCount} after {RetrySpan:g} due to {ExceptionType}: {ExceptionMessage}", 
                        retryCount, timeSpan, exception.GetType().Name, exception.Message);
                });
    }

    public async Task HandleClick(string payload)
    {
        this.logger.LogInformation("Handling click webhook");

        var payloadObject = JsonSerializer.Deserialize<EmailLinkClickedPayload>(payload);

        this.logger.LogInformation($"Handling click webhook for {payloadObject.to_email}");
        var email = payloadObject.to_email;
        if (email == null || string.IsNullOrWhiteSpace(email.ToString()))
        {
            throw new ArgumentNullException("to_email", "Email is required.");
        }

        await _retryPolicy.ExecuteAsync(async () => 
            await _smartLeadsEmailStatisticsRepository.UpsertEmailLinkClickedCount(payloadObject));

        await _retryPolicy.ExecuteAsync(async () => 
            await smartLeadsAllLeadsRepository.UpsertLeadFromEmailLinkClick(payloadObject));
    }

    public async Task HandleReply(string payload)
    {
        var payloadObject = JsonSerializer.Deserialize<EmailReplyPayload>(payload);
        var email = payloadObject.to_email;

        if (email == null || string.IsNullOrWhiteSpace(email.ToString()))
        {
            throw new ArgumentNullException("to_email", "Email is required.");
        }

        var replyAt = payloadObject.event_timestamp;


        await _retryPolicy.ExecuteAsync(async () => 
            await _messageHistoryRepository.UpsertEmailReply(payloadObject));

        await _retryPolicy.ExecuteAsync(async () => 
            await _smartleadsEmailStatisticsService.UpdateEmailReply(payloadObject));
    }

    public async Task HandleLeadCategoryUpdated(string payload)
    {
        var payloadObject = JsonSerializer.Deserialize<LeadCategoryUpdatedPayload>(payload);
        var email = payloadObject.lead_email;

        if (string.IsNullOrWhiteSpace(email.ToString()))
        {
            throw new ArgumentNullException("to_email", "Email is required.");
        }

        var lead = await this.smartLeadsAllLeadsRepository.GetByEmail(email.ToString());
        if (lead == null)
        {
            var campaignId = payloadObject.campaign_id;
            var account = await this.smartleadCampaignRepository.GetAccountByCampaignId(campaignId);

            var leadFromSmartLeads = await RetryHelper.ExecuteWithRetryAsync(async () =>
            {
                return await this.smartLeadsApiService.GetLeadByEmail(payloadObject.to_email, account.Id);
            });

            if (leadFromSmartLeads == null)
            {
                throw new ArgumentException("Email not found in both in our database or smartleads.");
            }

            await this.smartLeadsAllLeadsRepository.InsertLeadFromSmartleads(leadFromSmartLeads);
        }

        var leadCategoryName = payloadObject.lead_category.new_name;

        await this.smartLeadsAllLeadsRepository.UpdateLeadCategory(email.ToString(), leadCategoryName.ToString());
        // await this.automatedLeadsRepository.UpdateLeadCategory(email.ToString(), leadCategoryName.ToString());
    }

    public async Task HandleOpen(string payload)
    {
        this.logger.LogInformation("Handling email open webhook");

        var emailOpenPayload = JsonSerializer.Deserialize<EmailOpenPayload>(payload);

        this.logger.LogInformation($"Handling email open webhook for {emailOpenPayload.to_email}");
        var email = emailOpenPayload.to_email;
        if (email == null || string.IsNullOrWhiteSpace(email.ToString()))
        {
            throw new ArgumentNullException("to_email", "Email is required.");
        }

        var sequenceNumber = emailOpenPayload.sequence_number;


        await _retryPolicy.ExecuteAsync(async () => 
            await _smartLeadsEmailStatisticsRepository.UpsertEmailOpenCount(emailOpenPayload));
    }

    internal async Task HandleEmailSent(string payload)
    {
        this.logger.LogInformation("Handling email sent");

        var emailSentPayload = JsonSerializer.Deserialize<EmailSentPayload>(payload);

        this.logger.LogInformation($"Handling email sent webhook for {emailSentPayload.to_email}");
        var email = emailSentPayload.to_email;
        if (email == null || string.IsNullOrWhiteSpace(email.ToString()))
        {
            throw new ArgumentNullException("to_email", "Email is required.");
        }

        var sequenceNumber = emailSentPayload.sequence_number;

        // dbExecution.ExecuteInsideBackgroundTaskAsync(async () =>
        // {
        //     await _smartLeadsEmailStatisticsRepository.UpsertEmailSent(emailSentPayload);
        // });

        var campaign = await smartleadCampaignRepository.GetCampaignBdr(emailSentPayload.campaign_id.Value);
        if (campaign == null)
        {
            var account = await smartleadAccountRepository.GetAccountBySecretKey(emailSentPayload.secret_key);
            if (account == null)
            {
                throw new ArgumentException("Account not found for the provided secret key.");
            }

            var campaignDetails = await this.smartLeadsApiService.GetSmartLeadsCampaignById(emailSentPayload.campaign_id.Value, account.Id);
            await smartleadCampaignRepository.InsertCampaign(campaignDetails);
            await smartleadAccountRepository.InsertAccountCampaign(account.Id, campaignDetails.id);
        }

        await _retryPolicy.ExecuteAsync(async () =>
            await _messageHistoryRepository.UpsertEmailSent(emailSentPayload));

        await _retryPolicy.ExecuteAsync(async () =>
            await _smartleadsEmailStatisticsService.UpdateEmailSent(emailSentPayload));

        await _retryPolicy.ExecuteAsync(async () =>
            await smartLeadsAllLeadsRepository.UpsertLeadFromEmailSent(emailSentPayload));
    }

    internal async Task HandleEmailBounce(string payload)
    {
        var payloadObject = JsonSerializer.Deserialize<EmailBouncePayload>(payload);
        var email = payloadObject.to_email;

        if (string.IsNullOrWhiteSpace(email.ToString()))
        {
            throw new ArgumentNullException("to_email", "Email is required.");
        }

        var lead = await this.smartLeadsAllLeadsRepository.GetByEmail(email.ToString());
        if (lead == null)
        {
            var campaignId = payloadObject.campaign_id;
            var account = await this.smartleadCampaignRepository.GetAccountByCampaignId(campaignId);

            var leadFromSmartLeads = await RetryHelper.ExecuteWithRetryAsync(async () =>
            {
                return await this.smartLeadsApiService.GetLeadByEmail(payloadObject.to_email, account.Id);
            });

            if (leadFromSmartLeads == null)
            {
                throw new ArgumentException("Email not found in both in our database or smartleads.");
            }

            await this.smartLeadsAllLeadsRepository.InsertLeadFromSmartleads(leadFromSmartLeads);
        }

        await this.smartLeadsAllLeadsRepository.UpdateLeadCategory(email.ToString(), "Bounced");
        // await this.automatedLeadsRepository.UpdateLeadCategory(email.ToString(), "Bounced");
    }
}
