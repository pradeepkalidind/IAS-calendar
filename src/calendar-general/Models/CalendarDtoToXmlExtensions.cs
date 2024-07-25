using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Calendar.General.Models
{
    public static class CalendarDtoToXmlExtensions
    {
        public static void OutputXml<T>(this T dtoRoot, TextWriter textWriter)
        {
            var xmlTypeMapping = typeof(T);
            var serializer = new XmlSerializer(xmlTypeMapping);
            var xmlns = new XmlSerializerNamespaces();
            xmlns.Add(string.Empty, string.Empty);
            using (var writer = new XmlTextWriter(textWriter) { Formatting = Formatting.Indented })
            {
                serializer.Serialize(writer, dtoRoot, xmlns);
            }
        }
    }
}