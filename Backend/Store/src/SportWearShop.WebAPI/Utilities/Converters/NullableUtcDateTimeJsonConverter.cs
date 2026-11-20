using System.Text.Json;
using System.Text.Json.Serialization;

namespace SportWearShop.WebAPI.Utilities.Converters;

public sealed class NullableUtcDateTimeJsonConverter : JsonConverter<DateTime?>
{
    private static readonly UtcDateTimeJsonConverter Converter = new();

    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType == JsonTokenType.Null ? null : Converter.Read(ref reader, typeof(DateTime), options);

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value is { } dateTime)
        {
            Converter.Write(writer, dateTime, options);
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}

