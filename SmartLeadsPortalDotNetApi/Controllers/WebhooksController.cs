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

    [HttpPost("email-reply")]
    public async Task<IActionResult> EmailReply()
    {
        using var reader = new StreamReader(Request.Body);
        string payload = await reader.ReadToEndAsync();
        await this.webhooksRepository.InsertWebhook("EMAIL_REPLY", payload);
        await webhookService.HandleReply(payload);
        return Ok();
    }
}
