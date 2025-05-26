using Microsoft.AspNetCore.Mvc;
using SmartLeadsPortalDotNetApi.BackgroundTasks;

namespace SmartLeadsPortalDotNetApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BackgroundTaskController : ControllerBase
{
    private readonly WebhookBackgroundTaskQueue webhookBackgroundTaskQueue;

    public BackgroundTaskController(WebhookBackgroundTaskQueue webhookBackgroundTaskQueue)
    {
        this.webhookBackgroundTaskQueue = webhookBackgroundTaskQueue;
    }

    [HttpGet("tasks-status")]
    public Task<IActionResult> GetTasksStatus()
    {
        var statuses = this.webhookBackgroundTaskQueue.GetTasksStatus();
        return Task.FromResult<IActionResult>(Ok(statuses));
    }
}

