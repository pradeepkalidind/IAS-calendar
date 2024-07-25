using System.IO;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using Microsoft.AspNetCore.Mvc;

namespace Calendar.Tests.Unit
{
    public static class Extensions
    {
        public static SyndicationFeed ReadAsSyndicationFeed(this ContentResult result)
        {
            var respBody = result.Content;
            if (respBody == null)
                return null;

            var bytes = Encoding.UTF8.GetBytes(respBody);
            using (var stream = new MemoryStream(bytes))
            {
                using (var xmlReader = XmlReader.Create(stream))
                {
                    var formatter = new Atom10FeedFormatter();
                    formatter.ReadFrom(xmlReader);
                    return formatter.Feed;
                }
            }
        }
    }
}