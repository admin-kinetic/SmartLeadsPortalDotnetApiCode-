using System;
using System.Text.Json.Serialization;
using SmartLeadsPortalDotNetApi.Converters;

namespace SmartLeadsPortalDotNetApi.Aggregates.InboundCall;

public class UserInboundCompletedEvent : IInboundCallEvent
{
    [JsonPropertyName("type")]
    public string Type => "user_inbound_completed";
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
    [JsonPropertyName("queue_name")]
    public string? QueueName { get; set; }
    [JsonPropertyName("ring_group_name")]
    public string? RingGroupName { get; set; }
    [JsonPropertyName("timestamp")]
    public DateTime? Timestamp => CallStartAt?.AddSeconds(CallDuration ?? 0);
}
