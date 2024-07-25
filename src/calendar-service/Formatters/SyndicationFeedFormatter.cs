using System.IO;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;

namespace Calendar.Service.Formatters
{
    public static class SyndicationFeedFormatter
    {
        public const string ATOM_MEDIA_TYPE = "application/atom+xml";

        public static string Format(SyndicationFeed value)
        {
            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                NewLineHandling = NewLineHandling.Entitize,
                NewLineOnAttributes = true,
            };
            using var stream = new MemoryStream();
            using var xmlWriter = XmlWriter.Create(stream, settings);
            var rssFormatter = new Atom10FeedFormatter(value);
            rssFormatter.WriteTo(xmlWriter);
            xmlWriter.Flush();
            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }
}