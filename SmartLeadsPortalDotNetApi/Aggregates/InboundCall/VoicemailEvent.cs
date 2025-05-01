using System;
using System.Text.Json.Serialization;

namespace SmartLeadsPortalDotNetApi.Aggregates.InboundCall;

public class VoicemailEvent : IInboundCallEvent
{
    [JsonPropertyName("type")]
    public string Type => "ring_group_call";
    [JsonPropertyName("unique_call_id")]
    public string? UniqueCallId { get; set; }
    [JsonPropertyName("caller_id")]
    public string? CallerId { get; set; }
    [JsonPropertyName("dest_number")]
    public string? DestNumber { get; set; }
    [JsonPropertyName("recorded_at")]
    public DateTime? RecordedAt { get; set; }
    [JsonPropertyName("emails")]
    public string? Emails { get; set; }
    [JsonPropertyName("email_subject")]
    public string? EmailSubject { get; set; }
    [JsonPropertyName("email_message")]
    public string? EmailMessage { get; set; }
    [JsonPropertyName("audio_file")]
    public List<string>? AudioFile { get; set; } = new();
    [JsonPropertyName("timestamp")]
    public DateTime? Timestamp => RecordedAt;
}
