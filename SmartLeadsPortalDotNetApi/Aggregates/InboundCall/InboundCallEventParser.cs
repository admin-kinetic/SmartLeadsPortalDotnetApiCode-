using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartLeadsPortalDotNetApi.Aggregates.InboundCall;

public class InboundCallEventParser
{
    private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new CustomDateTimeConverter() }
    };

    public IInboundCallEvent ParseEvent(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var type = doc.RootElement.GetProperty("type").GetString();

        return type switch
        {
            "user_inbound" => JsonSerializer.Deserialize<UserInboundEvent>(json, Options),
            "user_user_inbound_answered" => JsonSerializer.Deserialize<UserInboundAnsweredEvent>(json, Options),
            "user_user_inbound_completed" => JsonSerializer.Deserialize<UserInboundCompletedEvent>(json, Options),
            "queue_call" => JsonSerializer.Deserialize<QueueCallEvent>(json, Options),
            "ring_group_call" => JsonSerializer.Deserialize<RingGroupCallEvent>(json, Options),
            "voicemail" => JsonSerializer.Deserialize<VoicemailEvent>(json, Options),
            "recording_user_inbound" => JsonSerializer.Deserialize<RecordingInboundEvent>(json, Options),
            _ => throw new NotSupportedException($"Event type {type} is not supported")
        };
    }
}

public class CustomDateTimeConverter : JsonConverter<DateTime?>
{
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string dateString = reader.GetString();
        if (dateString.ToLower() == "null")
        {
            return null;
        }

        if (DateTimeOffset.TryParse(dateString, out var dateTimeOffset))
        {
            return dateTimeOffset.DateTime;
        }

        throw new JsonException($"Unable to parse datetime: {dateString}");
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.HasValue ? value.Value.ToString("o") : null);
    }
}

