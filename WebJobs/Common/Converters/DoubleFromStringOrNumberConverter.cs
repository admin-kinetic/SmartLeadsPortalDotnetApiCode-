using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common.Converters;

public class DoubleFromStringOrNumberConverter : JsonConverter<double?>
{
    public override double? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Number => reader.GetDouble(),
            JsonTokenType.String when double.TryParse(reader.GetString(), out var value) => value,
            JsonTokenType.Null => null,
            _ => throw new JsonException("Invalid token type for double?")
        };
    }

    public override void Write(Utf8JsonWriter writer, double? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteNumberValue(value.Value);
        else
            writer.WriteNullValue();
    }
}
