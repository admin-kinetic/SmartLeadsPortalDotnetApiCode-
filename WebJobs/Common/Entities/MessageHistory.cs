using System.Text.Json.Serialization;

namespace Common.Entities;

public class MessageHistory
{
    public int? Id { get; set; }
    public Guid? StatsId { get; set; }
    public string? Type { get; set; }
    public string? MessageId { get; set; }
    public DateTime? Time { get; set; }
    public string? EmailBody { get; set; }
    public string? Subject { get; set; }
    public int? EmailSequenceNumber { get; set; }
    public int? OpenCount { get; set; }
    public int? ClickCount { get; set; }
}