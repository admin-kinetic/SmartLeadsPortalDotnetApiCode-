using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartLeadsPortalDotNetApi.Aggregates.OutboundCall;

public class OutboundCallEventParser
{
    private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new CustomDateTimeConverter() }
    };

    public IOutboundCallEvent ParseEvent(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var type = doc.RootElement.GetProperty("type").GetString();

        return type switch
        {
            "user_outbound" => JsonSerializer.Deserialize<UserOutboundEvent>(json, Options),
            "user_outbound_answered" => JsonSerializer.Deserialize<UserOutboundAnsweredEvent>(json, Options),
            "user_outbound_completed" => JsonSerializer.Deserialize<UserOutboundCompletedEvent>(json, Options),
            "recording_outbound" => JsonSerializer.Deserialize<RecordingOutboundEvent>(json, Options),
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

