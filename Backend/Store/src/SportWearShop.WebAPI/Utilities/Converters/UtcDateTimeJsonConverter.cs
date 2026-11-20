using System.Text.Json;
using System.Text.Json.Serialization;

namespace SportWearShop.WebAPI.Utilities.Converters;

public sealed class UtcDateTimeJsonConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String || !reader.TryGetDateTime(out var value))
        {
            throw new JsonException("Expected an ISO 8601 date and time.");
        }

        return AsUtc(value);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        => writer.WriteStringValue(AsUtc(value));

    private static DateTime AsUtc(DateTime value) => value.Kind switch
    {
        DateTimeKind.Utc => value,
        DateTimeKind.Local => value.ToUniversalTime(),
        // MySQL DATETIME values for the Store's UTC timestamps have no offset.
        _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
    };
}

