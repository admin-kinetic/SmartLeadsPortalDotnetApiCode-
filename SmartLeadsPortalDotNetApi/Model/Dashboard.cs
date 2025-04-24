namespace SmartLeadsPortalDotNetApi.Model
{
    public class Dashboard
    {
    }

    public class DashboardTodoTaskDue
    {
        public string? FullName { get; set; }
        public DateTime? Due{ get; set; }
        public int? OpenCount { get; set; }
    }

    public class DashboardTotalModel
    {
        public int Total { get; set; }
    }
}
