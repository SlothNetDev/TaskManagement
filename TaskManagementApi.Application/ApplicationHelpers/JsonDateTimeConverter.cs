using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TaskManagementApi.Application.ApplicationHelpers;

public class JsonDateTimeConverter : JsonConverter<DateTime>
{
    private readonly string _format;
    
    public JsonDateTimeConverter(string format = "yyyy-MM-ddTHH:mm:ssZ")
    {
        _format = format;
    }

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? dateString = reader.GetString();
        if (string.IsNullOrEmpty(dateString))
        {
            throw new JsonException("DateTime string cannot be null or empty.");
        }

        if (DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var result))
        {
            return result;
        }

        // Optionally, try parsing as DateTimeOffset for offset-based inputs
        if (DateTimeOffset.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTimeOffset))
        {
            return dateTimeOffset.UtcDateTime; // Convert to UTC DateTime
        }

        throw new JsonException($"Invalid DateTime format: {dateString}. Expected format: {_format}.");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(_format, CultureInfo.InvariantCulture));
    }
}