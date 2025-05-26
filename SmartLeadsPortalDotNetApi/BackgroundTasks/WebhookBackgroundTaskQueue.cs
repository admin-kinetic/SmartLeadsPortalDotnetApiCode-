using System.Collections.Concurrent;

namespace SmartLeadsPortalDotNetApi.BackgroundTasks;

public class WebhookBackgroundTaskQueue
{
    private readonly ConcurrentQueue<Func<CancellationToken, Task>> _workItems = new();
    private readonly ConcurrentDictionary<Guid, BackgroundTaskStatus> _taskStatuses = new();
    private readonly SemaphoreSlim _signal = new(0);

    public void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem, Guid taskId)
    {
        if (workItem == null)
        {
            throw new ArgumentNullException(nameof(workItem));
        }

        _workItems.Enqueue(workItem);
        _taskStatuses[taskId] = new BackgroundTaskStatus
        {
            TaskId = taskId,
            Status = TaskStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
        _signal.Release();
    }

    public async Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
    {
        await _signal.WaitAsync(cancellationToken);
        _workItems.TryDequeue(out var workItem);

        return workItem;
    }

    public List<BackgroundTaskStatus> GetTaskStatus(List<Guid> taskIds)
    {
        var statuses = new List<BackgroundTaskStatus>();
        foreach (var taskId in taskIds)
        {
            if (_taskStatuses.TryGetValue(taskId, out var status))
            {
                statuses.Add(status);
            }
        }

        return statuses;
    }

    public Dictionary<Guid, BackgroundTaskStatus> GetTasksStatus()
    {
        return _taskStatuses.ToDictionary();
    }

    public void UpdateTaskStatus(Guid taskId, TaskStatus status, string error = null)
    {
        if (_taskStatuses.TryGetValue(taskId, out var taskStatus))
        {
            taskStatus.Status = status;

            switch (status)
            {
                case TaskStatus.Running:
                    taskStatus.StartedAt = DateTime.UtcNow;
                    break;
                case TaskStatus.Completed:
                case TaskStatus.Failed:
                    taskStatus.CompletedAt = DateTime.UtcNow;
                    taskStatus.Error = error;
                    break;
            }
        }
    }
}
