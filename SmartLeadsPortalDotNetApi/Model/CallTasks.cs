namespace SmartLeadsPortalDotNetApi.Model
{
    public class CallTasks
    {
    }

    public class CallTasksUpdateParam
    {
        public Guid GuId { get; set; }
        public int StateId { get; set; }
        public int AssignedTo { get; set; }
        public string? Notes { get; set; }
    }
}
