using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Calendar.Service.Formatters
{
    public class CalendarFormatter
    {
        public const string CalendarMediaType = "application/vnd.calendar+xml";

        public static string Format(object value)
        {
            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                NewLineHandling = NewLineHandling.Entitize,
                NewLineOnAttributes = true,
            };

            var ms = new MemoryStream();
            var serializer = new XmlSerializer(value.GetType());
            using (var writer = XmlWriter.Create(ms, settings))
            {
                serializer.Serialize(writer, value);
            }

            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}