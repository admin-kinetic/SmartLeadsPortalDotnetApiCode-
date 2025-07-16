using System.Text.Json;
using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using SmartLeadsPortalDotNetApi.Repositories;
using SmartLeadsPortalDotNetApi.Services;

namespace SmartLeadsPortalDotNetApi.Controllers;

[ApiController]
[Route("api/webhooks")]
public class WebhooksController : ControllerBase
{
    private readonly WebhooksRepository webhooksRepository;
    private readonly WebhookService webhookService;
    private readonly ILogger<WebhooksController> logger;

    private readonly string[] webhookSecrets = {
        "0f47052b-e08b-488b-8ec3-dd949eec520a",
        "54251016-7093-4899-8111-63cc96e9757c"
    };

    public WebhooksController(WebhooksRepository webhooksRepository, WebhookService webhookService, ILogger<WebhooksController> logger)
    {
        this.webhooksRepository = webhooksRepository;
        this.webhookService = webhookService;
        this.logger = logger;
    }

    [HttpPost("email-sent")]
    public async Task<IActionResult> EmailSent()
    {
        using var reader = new StreamReader(Request.Body);
        string payload = await reader.ReadToEndAsync();

        using var jsonDoc = JsonDocument.Parse(payload);
        (bool flowControl, IActionResult value) = ValidateSecretKey(jsonDoc);
        if (!flowControl)
        {
            return value;
        }

        var eventType = jsonDoc.RootElement.GetProperty("event_type");
        if (eventType.ToString() != "EMAIL_SENT" ) {
            return BadRequest("incompatible event type");
        }

        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            try
            {
                await this.webhooksRepository.InsertWebhook("EMAIL_SENT", payload);
                await this.webhookService.HandleEmailSent(payload);
                scope.Complete();
                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing EMAIL_SENT webhook");
                // Transaction will automatically rollback
                throw;
            }
        }
    }

    [HttpPost("first-email-sent")]
    public async Task<IActionResult> FirstEmailSent()
    {
        using var reader = new StreamReader(Request.Body);
        string payload = await reader.ReadToEndAsync();

        using var jsonDoc = JsonDocument.Parse(payload);
        (bool flowControl, IActionResult value) = ValidateSecretKey(jsonDoc);
        if (!flowControl)
        {
            return value;
        }

        var eventType = jsonDoc.RootElement.GetProperty("event_type");
        // if (eventType.ToString() != "EMAIL_SENT" ) {
        //     return BadRequest("incompatible event type");
        // }

        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            try
            {
                await this.webhooksRepository.InsertWebhook("FIRST_EMAIL_SENT", payload);
                await this.webhookService.HandleEmailSent(payload);
                scope.Complete();
                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing EMAIL_SENT webhook");
                // Transaction will automatically rollback
                throw;
            }
        }
    }

    [HttpPost("email-open")]
    public async Task<IActionResult> EmailOpen()
    {
        using var reader = new StreamReader(Request.Body);
        string payload = await reader.ReadToEndAsync();

        using var jsonDoc = JsonDocument.Parse(payload);
        (bool flowControl, IActionResult value) = ValidateSecretKey(jsonDoc);
        if (!flowControl)
        {
            return value;
        }

        var eventType = jsonDoc.RootElement.GetProperty("event_type");
        if (eventType.ToString() != "EMAIL_OPEN" ) {
            return BadRequest("incompatible event type");
        }

        await this.webhooksRepository.InsertWebhook("EMAIL_OPEN", payload);
        await this.webhookService.HandleOpen(payload);
        return Ok();
    }

    // [HttpPost("click")]
    // public async Task<IActionResult> Click()
    // {
    //     using var reader = new StreamReader(Request.Body);
    //     string payload = await reader.ReadToEndAsync();
    //     await webhookService.HandleClick(payload);
    //     return Ok();
    // }

    [HttpPost("email-link-click")]
    public async Task<IActionResult> EmailLinkClick()
    {
        using var reader = new StreamReader(Request.Body);
        string payload = await reader.ReadToEndAsync();

        using var jsonDoc = JsonDocument.Parse(payload);
        (bool flowControl, IActionResult value) = ValidateSecretKey(jsonDoc);
        if (!flowControl)
        {
            return value;
        }
        
         var eventType = jsonDoc.RootElement.GetProperty("event_type");
        if (eventType.ToString() != "EMAIL_LINK_CLICK" ) {
            return BadRequest("incompatible event type");
        }

        await this.webhooksRepository.InsertWebhook("EMAIL_LINK_CLICK", payload);
        await webhookService.HandleClick(payload);
        return Ok();
    }

    [HttpPost("email-reply")]
    public async Task<IActionResult> EmailReply()
    {
        using var reader = new StreamReader(Request.Body);
        string payload = await reader.ReadToEndAsync();

        using var jsonDoc = JsonDocument.Parse(payload);
        (bool flowControl, IActionResult value) = ValidateSecretKey(jsonDoc);
        if (!flowControl)
        {
            return value;
        }
        
         var eventType = jsonDoc.RootElement.GetProperty("event_type");
        if (eventType.ToString() != "EMAIL_REPLY" ) {
            return BadRequest("incompatible event type");
        }

        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            try
            {
                await this.webhooksRepository.InsertWebhook("EMAIL_REPLY", payload);
                await webhookService.HandleReply(payload);
                scope.Complete();
                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing EMAIL_REPLY webhook");
                // Transaction will automatically rollback
                throw;
            }
        }
    }

    [HttpPost("lead-unsubscribed")]
    public async Task<IActionResult> LeadUnsubscribed()
    {
        using var reader = new StreamReader(Request.Body);
        string payload = await reader.ReadToEndAsync();
        await this.webhooksRepository.InsertWebhook("LEAD_UNSUBSCRIBED", payload);
        // await webhookService.HandleReply(payload);
        return Ok();
    }


    [HttpPost("lead-category-updated-prospect")]
    public async Task<IActionResult> LeadCategoryUpdatedProspect()
    {
        using var reader = new StreamReader(Request.Body);
        string payload = await reader.ReadToEndAsync();
        await this.webhooksRepository.InsertWebhook("LEAD_CATEGORY_UPDATED", payload);
        await webhookService.HandleLeadCategoryUpdated(payload);
        return Ok();
    }

    [HttpPost("lead-category-updated-invalid")]
    public async Task<IActionResult> LeadCategoryUpdatedInvalid()
    {
        using var reader = new StreamReader(Request.Body);
        string payload = await reader.ReadToEndAsync();
        await this.webhooksRepository.InsertWebhook("LEAD_CATEGORY_UPDATED", payload);
        await webhookService.HandleLeadCategoryUpdated(payload);
        return Ok();
    }

    [HttpPost("lead-category-updated-unavailable")]
    public async Task<IActionResult> LeadCategoryUpdatedUnavailable()
    {
        using var reader = new StreamReader(Request.Body);
        string payload = await reader.ReadToEndAsync();
        await this.webhooksRepository.InsertWebhook("LEAD_CATEGORY_UPDATED", payload);
        await webhookService.HandleLeadCategoryUpdated(payload);
        return Ok();
    }


    [HttpPost("email-bounce")]
    public async Task<IActionResult> EmailBounce()
    {
        using var reader = new StreamReader(Request.Body);
        string payload = await reader.ReadToEndAsync();
        await this.webhooksRepository.InsertWebhook("EMAIL_BOUNCE", payload);
        await webhookService.HandleEmailBounce(payload);
        return Ok();
    }

    [HttpPost("campaign-status-changed")]
    public async Task<IActionResult> CampaignStatusChanged()
    {
        using var reader = new StreamReader(Request.Body);
        string payload = await reader.ReadToEndAsync();
        await this.webhooksRepository.InsertWebhook("CAMPAIGN_STATUS_CHANGED", payload);
        // await webhookService.HandleReply(payload);
        return Ok();
    }

    [HttpPost("manual-step-reached")]
    public async Task<IActionResult> ManualStepReached()
    {
        using var reader = new StreamReader(Request.Body);
        string payload = await reader.ReadToEndAsync();
        await this.webhooksRepository.InsertWebhook("MANUAL_STEP_REACHED", payload);
        // await webhookService.HandleReply(payload);
        return Ok();
    }

    [HttpPost("untracked-replies")]
    public async Task<IActionResult> UntrackedReplies()
    {
        using var reader = new StreamReader(Request.Body);
        string payload = await reader.ReadToEndAsync();
        await this.webhooksRepository.InsertWebhook("UNTRACKED_REPLIES", payload);
        // await webhookService.HandleReply(payload);
        return Ok();
    }

    // [HttpPost("process-email-reply")]
    // public async Task<IActionResult> ProcessEmailReply()
    // {
    //     var emailReplyWebhooks = await this.webhooksRepository.GetEmailReplyWebhooks();
    //     foreach (var emailReplyWebhook in emailReplyWebhooks)
    //     {
    //         try
    //         {
    //             await webhookService.HandleReply(emailReplyWebhook);
    //         }
    //         catch (Exception ex)
    //         {
    //             // Log the error and continue processing other webhooks
    //             Console.WriteLine($"Error processing webhook: {ex.Message}");
    //             continue;
    //         }
    //     }
    //     return Ok();
    // }

    [HttpPost("process-lead-category-updated")]
    public async Task<IActionResult> ProcessLeadCategoryUpdated()
    {
        var emailReplyWebhooks = await this.webhooksRepository.GetLeadCategoryUpdated();
        foreach (var emailReplyWebhook in emailReplyWebhooks)
        {
            try
            {
                await webhookService.HandleLeadCategoryUpdated(emailReplyWebhook);
            }
            catch (Exception ex)
            {
                // Log the error and continue processing other webhooks
                this.logger.LogError($"Error processing webhook: {ex.Message}");
                continue;
            }
        }
        return Ok();
    }

    [HttpPost("process-lead-category-updated/{webhookId}")]
    public async Task<IActionResult> ProcessLeadCategoryUpdatedByWebhookId(int webhookId)
    {
        var emailReplyWebhooks = await this.webhooksRepository.GetLeadCategoryUpdatedByWebhookId(webhookId);
        foreach (var emailReplyWebhook in emailReplyWebhooks)
        {
            try
            {
                await webhookService.HandleLeadCategoryUpdated(emailReplyWebhook);
            }
            catch (Exception ex)
            {
                // Log the error and continue processing other webhooks
                this.logger.LogError($"Error processing webhook: {ex.Message}");
                continue;
            }
        }
        return Ok();
    }

    [HttpPost("process-email-bounce/{webhookId}")]
    public async Task<IActionResult> ProcessEmailBounceByWebhookId(int webhookId)
    {
        var emailReplyWebhooks = await this.webhooksRepository.GetLeadCategoryUpdatedByWebhookId(webhookId);
        foreach (var emailReplyWebhook in emailReplyWebhooks)
        {
            try
            {
                await webhookService.HandleEmailBounce(emailReplyWebhook);
            }
            catch (Exception ex)
            {
                // Log the error and continue processing other webhooks
                this.logger.LogError($"Error processing webhook: {ex.Message}");
                continue;
            }
        }
        return Ok();
    }

    private (bool flowControl, IActionResult value) ValidateSecretKey(JsonDocument jsonDoc)
    {
        // local test logs 0ms
        // this.logger.LogInformation("Validating secret key in webhook payload.");
        // var timer = System.Diagnostics.Stopwatch.StartNew();

        if (!jsonDoc.RootElement.TryGetProperty("secret_key", out JsonElement secretKeyElement))
        {
            return (flowControl: false, value: BadRequest("Missing secret_key in payload."));
        }
        string secretKey = secretKeyElement.GetString();
        if (string.IsNullOrEmpty(secretKey) || !webhookSecrets.Contains(secretKey))
        {
            this.logger.LogWarning("Invalid secret key provided in webhook payload.");
            return (flowControl: false, value: Unauthorized("Invalid secret key."));
        }

        // this.logger.LogInformation("Secret key validated successfully. It took {ElapsedMilliseconds} ms", timer.ElapsedMilliseconds);
        return (flowControl: true, value: null);
    }
}
