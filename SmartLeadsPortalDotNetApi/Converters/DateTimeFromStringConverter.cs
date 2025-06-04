using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartLeadsPortalDotNetApi.Converters;

public class DateTimeFromStringConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Read the JSON value as a string
        string dateTimeString = reader.GetString();

        // Parse the string to DateTime using DateTimeOffset for robustness
        // This handles both "2025-05-21T01:36:12.000Z" and "2025-05-21T01:36:04.296+00:00"
        DateTimeOffset dateTimeOffset = DateTimeOffset.Parse(dateTimeString);

        // Return the UTC DateTime representation
        return dateTimeOffset.UtcDateTime;
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        // Serialize the DateTime in ISO 8601 format with 'Z' suffix for UTC
        writer.WriteStringValue(value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
    }
}