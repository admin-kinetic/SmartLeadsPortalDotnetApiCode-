using Common.Converters;
using System;
using System.Text.Json.Serialization;

namespace ImportSmartLeadStatistics.Entities;

public class SmartLeadsEmailStatistics
{
    public int Id { get; set; }
    public Guid GuId { get; set; }
    public double LeadId { get; set; }
    public string? LeadEmail { get; set; }
    public string? LeadName { get; set; }
    public int? SequenceNumber { get; set; }
    public string? EmailSubject { get; set; }
    [JsonConverter(typeof(DateTimeFromStringConverter))]
    public DateTime? SentTime { get; set; }
    [JsonConverter(typeof(DateTimeFromStringConverter))]
    public DateTime? OpenTime { get; set; }
    [JsonConverter(typeof(DateTimeFromStringConverter))]
    public DateTime? ClickTime { get; set; }
    [JsonConverter(typeof(DateTimeFromStringConverter))]
    public DateTime? ReplyTime { get; set; }
    public int? OpenCount { get; set; }  
    public int? ClickCount { get; set; }
}
