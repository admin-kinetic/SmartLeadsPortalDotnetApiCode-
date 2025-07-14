namespace SmartLeadsPortalDotNetApi.Entities
{
    public class CallsReport
    {
        public string? UniqueCallId { get; set; }
        public string? CallType { get; set; }
        public string? CallerId { get; set; }
        public string? UserName { get; set; }
        public string? DestNumber { get; set; }
        public DateTime? CallStartAt { get; set; }
        public int? CallDuration { get; set; }
        public int? ConversationDuration { get; set; }
        public string? AzureStorageCallRecordingLink { get; set; }
        public string? CallerName { get; set; }
    }

    public class CallsParam
    {
        public string? Search { get; set; }
        public string? Bdr { get; set; }
        public int? Type { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }

    public class BdrDropdown
    {
        public string? UserName { get; set; }
    }
}
