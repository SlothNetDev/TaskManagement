using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TaskManagementApi.Application.ApplicationHelpers
{
    /// <summary>
    /// for conversion of Datetime into Json
    /// </summary>
    public sealed class DatetimeConversion : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
             try 
             {
                 return DateTime.ParseExact(
                     reader.GetString()!,
                     "O", // ISO 8601 round-trip format
                     CultureInfo.InvariantCulture,
                     DateTimeStyles.RoundtripKind);
             }
             catch (FormatException ex)
             {
                 throw new JsonException("Invalid ISO 8601 DateTime format", ex);
             }
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("O"));
        }
    }
}
