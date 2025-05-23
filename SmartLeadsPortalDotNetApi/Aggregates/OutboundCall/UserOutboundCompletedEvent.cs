using System;
using System.Text.Json.Serialization;
using SmartLeadsPortalDotNetApi.Converters;

namespace SmartLeadsPortalDotNetApi.Aggregates.OutboundCall;

public class UserOutboundCompletedEvent : IOutboundCallEvent
{
    [JsonPropertyName("type")]
    public string Type => "user_outbound_completed";
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
    [JsonPropertyName("call_start_at")]
    public DateTime? CallStartAt { get; set; }
    [JsonPropertyName("connected_at")]
    public DateTime? ConnectedAt { get; set; }
    [JsonPropertyName("call_duration")]
    public int? CallDuration { get; set; }
    [JsonPropertyName("conversation_duration")]
    [JsonConverter(typeof(IntFromStringOrNumberConverter))]
    public int? ConversationDuration { get; set; }
    [JsonPropertyName("timestamp")]
    public DateTime? Timestamp => CallStartAt?.AddSeconds(CallDuration.Value);
}
