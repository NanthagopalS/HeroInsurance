using Newtonsoft.Json;

namespace Identity.API.Helpers
{
    /// <summary>
    /// Responsible for datetimeconversion as by default ISODate is supported.
    /// </summary>
    public class CustomDateTimeConverter : JsonConverter<DateTime?>
    {
        private const string Format = "MM/dd/yyyy HH:mm:ss";

        /// <summary>
        /// Reads and converts the JSON to type Datetime.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="hasExistingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override DateTime? ReadJson(JsonReader reader, Type objectType, DateTime? existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            var dateTimeStringValue = reader.Value.ToString();
            DateTime dateTime = DateTime.Parse(dateTimeStringValue);
            return dateTime;
        }

        /// <summary>
        /// Writes a specified datetime value as JSON.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, DateTime? value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Value.ToString(Format));
        }
    }
}
