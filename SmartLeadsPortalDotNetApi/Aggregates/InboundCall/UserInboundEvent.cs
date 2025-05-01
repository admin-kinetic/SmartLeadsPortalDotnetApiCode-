using System;
using System.Text.Json.Serialization;

namespace SmartLeadsPortalDotNetApi.Aggregates.InboundCall;

public class UserInboundEvent : IInboundCallEvent
{
    [JsonPropertyName("type")]
    public string Type => "user_inbound";
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
    [JsonPropertyName("queue_name")]
    public string? QueueName { get; set; }
    [JsonPropertyName("ring_group_name")]
    public string? RingGroupName { get; set; }
    [JsonPropertyName("timestamp")]
    public DateTime? Timestamp => CallStartAt;
}