using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartLeadsPortalDotNetApi.Converters;

public class StringFromStringOrNumberConverter : JsonConverter<string?>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.String => reader.GetString(),
            JsonTokenType.Number => reader.TryGetInt32(out int intVal) 
                ? intVal.ToString() 
                : reader.GetDouble().ToString(),
            JsonTokenType.Null => null,
            _ => throw new JsonException("Invalid token type for string?")
        };
    }

    public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
    {
        if (value != null)
            writer.WriteStringValue(value);
        else
            writer.WriteNullValue();
    }
}
