using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MWDataMigrationApp
{
    public class ProjectData
    {
        public long key{ get; set; }
        public int proId { get; set; }
        public DateTime? st { get; set; }
        public DateTime? end { get; set; }
    }
    public class DateTimeConverter : JsonConverter<DateTime?>
    {
        private const string DateFormat = "yyyy-MM-ddTHH:mm:ss";

        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String && DateTime.TryParse(reader.GetString(), out var date))
            {
                return date;
            }
            return null;
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value?.ToString(DateFormat));
        }
    }

    public class ProjectDataConverter : JsonConverter<(int, DateTime?, DateTime?)>
    {
        public override (int, DateTime?, DateTime?) Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException();
            }

            reader.Read();
            int proId = reader.GetInt32();

            reader.Read();
            DateTime? st = reader.GetString() != "NaT" ? reader.GetDateTime() : (DateTime?)null;

            reader.Read();
            DateTime? end = reader.GetString() != "NaT" ? reader.GetDateTime() : (DateTime?)null;

            reader.Read();
            if (reader.TokenType != JsonTokenType.EndArray)
            {
                throw new JsonException();
            }

            return (proId, st, end);
        }

        public override void Write(Utf8JsonWriter writer, (int, DateTime?, DateTime?) value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            writer.WriteNumberValue(value.Item1);
            writer.WriteStringValue(value.Item2?.ToString("yyyy-MM-dd HH:mm:ss") ?? "NaT");
            writer.WriteStringValue(value.Item3?.ToString("yyyy-MM-dd HH:mm:ss") ?? "NaT");
            writer.WriteEndArray();
        }
    }
}
