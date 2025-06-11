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
        public int? Total { get; set; }
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
    public class DashboardEmailStatistics
    {
        public int? TotalCount { get; set; }

    }

    public class DashboardDropdownList
    {
        public int Id { get; set; }
        public Guid GuId { get; set; }
        public string? ValueName { get; set; }
        public bool? IsActive { get; set; }
    }
    public class DashboardFilterModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Bdr { get; set; }
        public string? CreatedBy { get; set; }
        public string? QaBy { get; set; }
        public int? CampaignId { get; set; }
    }

    public class DashboardAnalyticsTotalSent
    {
        public int? TotalSent { get; set; }

    }
    public class DashboardAnalyticsTotalReplied
    {
        public int? TotalReplied { get; set; }

    }
    public class DashboardAnalyticsTotalOpened
    {
        public int? TotalOpened { get; set; }

    }
    public class DashboardAnalyticsTotalUniqueOpened
    {
        public int? TotalUniqueOpened { get; set; }

    }
    public class DashboardAnalyticsTotalBounced
    {
        public int? TotalBounced { get; set; }

    }
    public class DashboardAnalyticsTotalInterested
    {
        public int? TotalInterested { get; set; }

    }
    public class DashboardAnalyticsTotalExported
    {
        public int? TotalExported { get; set; }

    }
    public class DashboardAnalyticsTotalOutOfOffice
    {
        public int? TotalOutOfOffice { get; set; }

    }
    public class DashboardAnalyticsTotalIncorrectContact
    {
        public int? TotalIncorrectContact { get; set; }

    }
}
