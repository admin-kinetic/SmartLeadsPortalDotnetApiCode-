namespace SmartLeadsPortalDotNetApi.BackgroundTasks;

public class WebhookBackgroundService : IHostedService
{
    private readonly WebhookBackgroundTaskQueue _taskQueue;
    private Task _backgroundTask;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly ILogger<WebhookBackgroundService> logger;

    public WebhookBackgroundService(WebhookBackgroundTaskQueue taskQueue, ILogger<WebhookBackgroundService> logger)
    {
        _taskQueue = taskQueue;
        this.logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _backgroundTask = Task.Run(() => ProcessTaskQueueAsync(_cancellationTokenSource.Token));
        return Task.CompletedTask;
    }

    private async Task ProcessTaskQueueAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var workItem = await _taskQueue.DequeueAsync(cancellationToken);

            try
            {
                await workItem(cancellationToken);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error processing background task");
            }
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _cancellationTokenSource.Cancel();
        await Task.WhenAny(_backgroundTask, Task.Delay(Timeout.Infinite, cancellationToken));
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
    }
}
