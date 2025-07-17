using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

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
            JsonTokenType.StartArray => ReadStringArray(ref reader),
            JsonTokenType.Null => null,
            _ => throw new JsonException("Invalid token type for string?")
        };
    }

    private string? ReadStringArray(ref Utf8JsonReader reader)
    {
        var strings = new List<string>();
        
        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                strings.Add(reader.GetString()!);
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                strings.Add(reader.TryGetInt32(out int intVal) 
                    ? intVal.ToString() 
                    : reader.GetDouble().ToString());
            }
        }

        return strings.Count > 0 ? string.Join(",", strings) : null;
    }

    public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
    {
        if (value != null)
            writer.WriteStringValue(value);
        else
            writer.WriteNullValue();
    }
}
