using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using NHibernate.Linq;

namespace Calendar.Model.Compact
{
    public class UserFeed
    {
        private readonly Guid id;
        private readonly DateTime startTime;
        private readonly DateTime endTime;
        private const string ContentTemplate = "<IASPlatformUser Id=\"{0}\"/><Days>{1}</Days>";
        private const string DayTemplate = "<Day>{0}</Day>";
        private const string TitleTemplate = "Calendar Changed of User [{0}]";
        private const string DateFormat = "yyyy-MM-dd";

        public UserFeed(Guid id, Guid userId, DateTime startTime, DateTime endTime)
        {
            this.id = id;
            this.startTime = startTime;
            this.endTime = endTime;
            UserId = userId;
            dates = new List<DateTime>();
        }

        public Guid UserId { get; private set; }

        public DateTime LastUpdatedTime
        {
            get { return new DateTime(timestamp, DateTimeKind.Utc); }
        }

        public void AddDay(DateTime date, long updatedTime)
        {
            if(!dates.Contains(date))
            {
                dates.Add(date);
            }
            if (updatedTime > timestamp)
            {
                timestamp = updatedTime;
            }
        }

        public SyndicationItem ToSyndicationItem(string prefixUrl)
        {
            var item = new SyndicationItem
                           {
                               Title = new TextSyndicationContent(Title),
                               Content = new TextSyndicationContent(Content),
                               LastUpdatedTime = LastUpdatedTime,
                               Id = id.ToString(),
                           };
            
            FeedEntryLinks.ToList().ForEach(link => item.Links.Add(ToSyndicationLink(link,prefixUrl)));
            return item;
        }

        private string Title
        {
            get
            {
                return string.Format(TitleTemplate, UserId);
            }
        }

        private string Content
        {
            get
            {
                var days = dates.Select(d => string.Format(DayTemplate, d.ToString(DateFormat))).ToArray();
                return string.Format(ContentTemplate, UserId, String.Join("", days));
            }
        }

        private IEnumerable<FeedEntryLink> FeedEntryLinks
        {
            get { return new List<FeedEntryLink> { AlternativeLink, ForDayLink }; }
        }

        private FeedEntryLink alternativeLink;

        private FeedEntryLink AlternativeLink
        {
            get
            {
                if (alternativeLink == null)
                {
                    var uri = string.Format(FeedEntryLink.AlternativeUrlTemplate, UserId);
                    alternativeLink = new FeedEntryLink(FeedEntryLink.AlternativeRelationShip, uri);
                }
                return alternativeLink;
            }
        }

        private FeedEntryLink forDayLink;

        private FeedEntryLink ForDayLink
        {
            get
            {
                if (forDayLink == null)
                {
                    var uri = string.Format(FeedEntryLink.ForDayUrlTemplate, UserId, startTime.Ticks,endTime.Ticks);
                    forDayLink = new FeedEntryLink(FeedEntryLink.ForDayRelationShip, uri);
                }
                return forDayLink;
            }
        }

        private readonly List<DateTime> dates;

        private long timestamp = 0;

        private static SyndicationLink ToSyndicationLink(FeedEntryLink link, string prefixUrl)
        {
            return new SyndicationLink
            {
                Uri = new Uri(prefixUrl + link.Uri),
                RelationshipType = link.RelationshipType,
                MediaType = link.MediaType
            };
        }
    }
}