using System;
using System.Text.Json;
using SmartLeadsPortalDotNetApi.Repositories;

namespace SmartLeadsPortalDotNetApi.Services;

public class WebhookService
{
    private readonly AutomatedLeadsRepository automatedLeadsRepository;
    private readonly WebhooksRepository webhooksRepository;
    private readonly LeadClicksRepository leadClicksRepository;
    private readonly ILogger<WebhookService> logger;

    public WebhookService(
        AutomatedLeadsRepository automatedLeadsRepository, 
        WebhooksRepository webhooksRepository,
        LeadClicksRepository leadClicksRepository,
        ILogger<WebhookService> logger)
    {
        this.automatedLeadsRepository = automatedLeadsRepository;
        this.webhooksRepository = webhooksRepository;
        this.leadClicksRepository = leadClicksRepository;
        this.logger = logger;
    }

    public async Task HandleClick(Dictionary<string, object> payload)
    {
        this.logger.LogInformation("Handling click webhook");
        var email = payload["to_email"];
        if(email == null || string.IsNullOrWhiteSpace(email.ToString()))
        {
            throw new ArgumentNullException("to_email", "Email is required.");
        }

        var lead = await this.automatedLeadsRepository.GetByEmail(email.ToString());
        if(lead == null)
        {
            throw new ArgumentException("Email not found in leads.");
        }

        await this.leadClicksRepository.UpsertById(lead.Id);

        var serializedPayload = JsonSerializer.Serialize(payload);
        await this.webhooksRepository.InsertWebhook(serializedPayload);
        this.logger.LogInformation("Completed handling click webhook");
    }
}
