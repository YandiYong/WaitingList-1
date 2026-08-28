using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace waitingList.Converters
{
    /// <summary>
    /// Keeps every API visit date in the system format: dd/MM/yyyy.
    /// </summary>
    public sealed class StrictDateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        public const string dateFormat = "dd/MM/yyyy";

        public override DateOnly Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            var value = reader.GetString();

            if (value is null ||
                !DateOnly.TryParseExact(
                    value,
                    dateFormat,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var date))
            {
                throw new JsonException($"The date must use the format {dateFormat}.");
            }

            return date;
        }

        public override void Write(
            Utf8JsonWriter writer,
            DateOnly value,
            JsonSerializerOptions options)
        {
            writer.WriteStringValue(
                value.ToString(dateFormat, CultureInfo.InvariantCulture));
        }
    }
}
