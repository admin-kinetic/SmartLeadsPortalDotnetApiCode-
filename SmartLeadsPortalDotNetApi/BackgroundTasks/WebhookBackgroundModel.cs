namespace SmartLeadsPortalDotNetApi.BackgroundTasks;

public class BackgroundTask
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class BackgroundTaskStatus
{
    public Guid TaskId { get; set; }
    public TaskStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string Error { get; set; }
}

public enum TaskStatus
{
    Pending,
    Running,
    Completed,
    Failed
}
