using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Data.SqlClient;
using SmartLeadsPortalDotNetApi.Database;
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
    private readonly ILogger<WebhookService> logger;

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
        this.logger = logger;
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

        await dbExecution.ExecuteWithRetryAsync(async () =>
        {
            await _smartLeadsEmailStatisticsRepository.UpsertEmailLinkClickedCount(payloadObject);
        });

        await dbExecution.ExecuteWithRetryAsync(async () =>
        {
            await smartLeadsAllLeadsRepository.UpsertLeadFromEmailLinkClick(payloadObject);
        });
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


        await dbExecution.ExecuteWithRetryAsync(async () =>
        {
            await _messageHistoryRepository.UpsertEmailReply(payloadObject);
        });

        await dbExecution.ExecuteWithRetryAsync(async () =>
        {
            await _smartleadsEmailStatisticsService.UpdateEmailReply(payloadObject);
        });
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


        await dbExecution.ExecuteWithRetryAsync(async () =>
        {
            await _smartLeadsEmailStatisticsRepository.UpsertEmailOpenCount(emailOpenPayload);
            return true;
        });
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

        await dbExecution.ExecuteWithRetryAsync(async () =>
        {
            await _messageHistoryRepository.UpsertEmailSent(emailSentPayload);
        });

        await dbExecution.ExecuteWithRetryAsync(async () =>
        {
            await _smartleadsEmailStatisticsService.UpdateEmailSent(emailSentPayload);
        });

        // await dbExecution.ExecuteWithRetryAsync(async () =>
        // {
        //     await smartLeadsAllLeadsRepository.UpsertLeadFromEmailSent(emailSentPayload);
        //     return true;
        // });
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
