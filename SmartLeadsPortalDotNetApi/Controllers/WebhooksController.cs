using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SmartLeadsPortalDotNetApi.Repositories;
using SmartLeadsPortalDotNetApi.Services;

namespace SmartLeadsPortalDotNetApi.Controllers;

[ApiController]
[Route("api/webhooks")]
public class WebhooksController: ControllerBase
{
    private readonly WebhooksRepository webhooksRepository;
    private readonly WebhookService webhookService;

    public WebhooksController(WebhooksRepository webhooksRepository, WebhookService webhookService)
    {
        this.webhooksRepository = webhooksRepository;
        this.webhookService = webhookService;
    }

    [HttpPost("click")]
    public async Task<IActionResult> Click()
    {
        using var reader = new StreamReader(Request.Body);
        string payload = await reader.ReadToEndAsync();
        await webhookService.HandleClick(payload);
        return Ok();
    }

    
    [HttpPost("email-open")]
    public async Task<IActionResult> EmailClick()
    {
        using var reader = new StreamReader(Request.Body);
        string payload = await reader.ReadToEndAsync();
        await this.webhooksRepository.InsertWebhook("EMAIL_OPEN", payload);
        return Ok();
    }

    [HttpPost("email-sent")]
    public async Task<IActionResult> EmailSent()
    {
        using var reader = new StreamReader(Request.Body);
        string payload = await reader.ReadToEndAsync();
        await this.webhooksRepository.InsertWebhook("EMAIL_SENT", payload);
        return Ok();
    }

    [HttpPost("email-reply")]
    public async Task<IActionResult> EmailReply()
    {
        using var reader = new StreamReader(Request.Body);
        string payload = await reader.ReadToEndAsync();
        await this.webhooksRepository.InsertWebhook("EMAIL_REPLY", payload);
        await webhookService.HandleReply(payload);
        return Ok();
    }

    [HttpPost("email-bounce")]
    public async Task<IActionResult> EmailBounce()
    {
        using var reader = new StreamReader(Request.Body);
        string payload = await reader.ReadToEndAsync();
        await this.webhooksRepository.InsertWebhook("EMAIL_BOUNCE", payload);
        await webhookService.HandleReply(payload);
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
}
