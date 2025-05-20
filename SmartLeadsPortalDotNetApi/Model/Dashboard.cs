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

    public class DashboardEmailCampaignModel
    {
        public string? LeadsUser { get; set; }
        public int Total { get; set; }
    }
    public class DashboardDateParameter
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
    public class DashboardSmartLeadCampaignsActive
    {
        public int Id { get; set; }
    }

    public class DashboardAutomatedCampaignLeadgen
    {
        public string? ExportedDate { get; set; }
        public string? LeadGen { get; set; }
        public int? TotalCount { get; set; }

    }
    public class DashboardAutomatedCampaign
    {
        public string? ExportedDate { get; set; }
        public int? TotalCount { get; set; }

    }
}
