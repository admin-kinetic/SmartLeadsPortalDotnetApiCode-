using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartLeadsPortalDotNetApi.Converters;

public class IntFromStringOrNumberConverter : JsonConverter<int?>
{
    public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Number => reader.GetInt32(),
            JsonTokenType.String when int.TryParse(reader.GetString(), out var value) => value,
            JsonTokenType.Null => null,
            _ => throw new JsonException("Invalid token type for int?")
        };
    }

    public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteNumberValue(value.Value);
        else
            writer.WriteNullValue();
    }
}
