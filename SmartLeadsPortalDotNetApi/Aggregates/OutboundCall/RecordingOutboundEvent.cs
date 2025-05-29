using System;
using System.Text.Json.Serialization;

namespace SmartLeadsPortalDotNetApi.Aggregates.OutboundCall;

public class RecordingOutboundEvent : IOutboundCallEvent
{
    [JsonPropertyName("type")]
    public string Type => "recording_outbound";
    [JsonPropertyName("unique_call_id")]
    public string? UniqueCallId { get; set; }
    [JsonPropertyName("caller_id")]
    public string? CallerId { get; set; }
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }
    [JsonPropertyName("user_number")]
    public string? UserNumber { get; set; }
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
    [JsonPropertyName("call_recording_link")]
    public string CallRecordingLink { get; set; }
    [JsonPropertyName("timestamp")]
    public DateTime? Timestamp => RecordedAt;
}
