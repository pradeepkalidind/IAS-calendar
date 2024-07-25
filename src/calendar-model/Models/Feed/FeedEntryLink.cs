using System;

namespace Calendar.Models.Feed
{
    public class FeedEntryLink
    {
        public const string MediaTypeTemplate = "application/vnd.calendar+xml";
        public const string AlternativeRelationShip = "alternative";
        public const string ForDayRelationShip = "forDay";
        public const string AlternativeUrlTemplate = "/IASPlatformUser/{0}/Calendar";
        public const string ForDayUrlTemplate = "/IASPlatformUser/{0}/Calendar/{1}/{1}";

        public FeedEntryLink(string relationShipType, string uri)
        {
            Id = Guid.NewGuid();
            MediaType = MediaTypeTemplate;
            RelationshipType = relationShipType;
            Uri = uri;
        }

        public Guid Id { get; private set; }

        public string RelationshipType { get; private set; }

        public string MediaType { get; private set; }

        public string Uri { get; private set; }

    }
}