using System;
using System.Text.Json.Serialization;

namespace SmartLeadsPortalDotNetApi.Aggregates.OutboundCall;

public class UserOutboundAnsweredEvent : IOutboundCallEvent
{
    [JsonPropertyName("type")]
    public string Type => "user_outbound_answered";
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
    [JsonPropertyName("timestamp")]
    public DateTime? Timestamp => ConnectedAt;
}
