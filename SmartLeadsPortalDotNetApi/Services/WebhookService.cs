using System;
using System.Text.Json;
using System.Text.Json.Nodes;
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
    private readonly ILogger<WebhookService> logger;

    public WebhookService(
        AutomatedLeadsRepository automatedLeadsRepository,
        WebhooksRepository webhooksRepository,
        LeadClicksRepository leadClicksRepository,
        SmartLeadsExportedContactsRepository smartLeadsExportedContactsRepository,
        SmartLeadsEmailStatisticsRepository smartLeadsEmailStatisticsRepository,
        MessageHistoryRepository messageHistoryRepository,
        SmartLeadsAllLeadsRepository smartLeadsAllLeadsRepository,
        ILogger<WebhookService> logger)
    {
        this.automatedLeadsRepository = automatedLeadsRepository;
        this.webhooksRepository = webhooksRepository;
        this.leadClicksRepository = leadClicksRepository;
        _smartLeadsEmailStatisticsRepository = smartLeadsEmailStatisticsRepository;
        _messageHistoryRepository = messageHistoryRepository;
        this.smartLeadsAllLeadsRepository = smartLeadsAllLeadsRepository;
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

        await _smartLeadsEmailStatisticsRepository.UpsertEmailLinkClickedCount(payloadObject);

        var lead = await this.automatedLeadsRepository.GetByEmail(email.ToString());
        if (lead == null)
        {
            throw new ArgumentException("Email not found in leads.");
        }

        await this.leadClicksRepository.UpsertClickCountById(lead.Id);


        this.logger.LogInformation("Completed handling click webhook");
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
        await _messageHistoryRepository.UpsertEmailReply(payloadObject);

        // await this.automatedLeadsRepository.UpdateReply(email.ToString(), replyAt.ToString());
    }

    public async Task HandleLeadCategoryUpdated(string payload)
    {
        var payloadObject = JsonSerializer.Deserialize<JsonElement>(payload);
        var email = payloadObject.GetProperty("lead_email");

        if (string.IsNullOrWhiteSpace(email.ToString()))
        {
            throw new ArgumentNullException("to_email", "Email is required.");
        }

        var leadCategoryName = payloadObject.GetProperty("lead_category").GetProperty("new_name");

        await this.automatedLeadsRepository.UpdateLeadCategory(email.ToString(), leadCategoryName.ToString());
    }

    public async Task HandleOpen(string payload)
    {
        this.logger.LogInformation("Handling open webhook");

        var emailOpenPayload = JsonSerializer.Deserialize<EmailOpenPayload>(payload);

        this.logger.LogInformation($"Handling open webhook for {emailOpenPayload.to_email}");
        var email = emailOpenPayload.to_email;
        if (email == null || string.IsNullOrWhiteSpace(email.ToString()))
        {
            throw new ArgumentNullException("to_email", "Email is required.");
        }

        var sequenceNumber = emailOpenPayload.sequence_number;

        await _smartLeadsEmailStatisticsRepository.UpsertEmailOpenCount(emailOpenPayload);

        // var lead = await this.automatedLeadsRepository.GetByEmail(email.ToString());
        // if (lead == null)
        // {
        //     throw new ArgumentException("Email not found in leads.");
        // }

        // await this.leadClicksRepository.UpsertOpenCountById(lead.Id);

        // this.logger.LogInformation("Completed handling opwn webhook");
    }

    internal async Task HandleEmailSent(string payload)
    {
        this.logger.LogInformation("Handling open webhook");

        var emailSentPayload = JsonSerializer.Deserialize<EmailSentPayload>(payload);

        this.logger.LogInformation($"Handling open webhook for {emailSentPayload.to_email}");
        var email = emailSentPayload.to_email;
        if (email == null || string.IsNullOrWhiteSpace(email.ToString()))
        {
            throw new ArgumentNullException("to_email", "Email is required.");
        }

        var sequenceNumber = emailSentPayload.sequence_number;

        await _smartLeadsEmailStatisticsRepository.UpsertEmailSent(emailSentPayload);
        await _messageHistoryRepository.UpsertEmailSent(emailSentPayload);
        await smartLeadsAllLeadsRepository.UpsertLeadFromEmailSent(emailSentPayload);


    }

    internal async Task HandleEmailBounce(string payload)
    {
        var payloadObject = JsonSerializer.Deserialize<JsonElement>(payload);
        var email = payloadObject.GetProperty("to_email");

        if (string.IsNullOrWhiteSpace(email.ToString()))
        {
            throw new ArgumentNullException("to_email", "Email is required.");
        }

        await this.automatedLeadsRepository.UpdateLeadCategory(email.ToString(), "Sender Originated Bounce");
    }

    
}
