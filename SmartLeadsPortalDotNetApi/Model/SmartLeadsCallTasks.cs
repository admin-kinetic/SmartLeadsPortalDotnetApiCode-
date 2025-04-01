﻿namespace SmartLeadsPortalDotNetApi.Model
{
    public class SmartLeadsCallTasks
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public int LeadId { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public int? SequenceNumber { get; set; }
        public string? CampaignName { get; set; }
        public string? SubjectName { get; set; }
        public int? OpenCount { get; set; }
        public int? ClickCount { get; set; }
        public string? CallState { get; set; }
    }

    public class SmartLeadsEmailStatistics
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public int LeadId { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public int? SequenceNumber { get; set; }
        public string? CampaignName { get; set; }
        public string? SubjectName { get; set; }
        public int? OpenCount { get; set; }
        public int? ClickCount { get; set; }
        public int? CallStateId { get; set; }
    }
    public class SmartLeadsCallTasksResponseModel<T>
    {
        public List<T>? Items { get; set; }
        public int Total { get; set; }
    }
    public class SmartLeadsCallTasksRequest
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
