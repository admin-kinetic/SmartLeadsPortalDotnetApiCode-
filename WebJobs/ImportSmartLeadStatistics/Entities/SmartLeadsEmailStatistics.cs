using System;

namespace ImportSmartLeadStatistics.Entities;

public class SmartLeadsEmailStatistics
{
    public int Id { get; set; }
    public Guid GuId { get; set; }
    public int LeadId { get; set; }
    public string? LeadEmail { get; set; }
    public int? SequenceNumber { get; set; }
    public string? EmailSubject { get; set; }
    public DateTime? SentTime { get; set; }
    public DateTime? OpenTime { get; set; }  
    public DateTime? ClickTime { get; set; }
    public DateTime? ReplyTime { get; set; }
    public int? OpenCount { get; set; }  
    public int? ClickCount { get; set; }
}
