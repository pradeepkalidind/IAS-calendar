using System;
using System.Collections.Generic;

namespace Calendar.Models.Feed
{
    public class FeedArchivedEntry : FeedEntryBase
    {
        public FeedArchivedEntry():base()
        {
        }

        public FeedArchivedEntry(Guid userId, DateTime forDay, long timestamp):
            base(userId,forDay,timestamp)
        {
            
        }
    }

    public abstract class FeedEntryBase
    {
        private string title;
        private string content;
        private FeedEntryLink alternativeLink;
        private FeedEntryLink forDayLink;

        protected FeedEntryBase()
        {
            Id = Guid.NewGuid();
        }

        protected FeedEntryBase(Guid userId, DateTime forDay, long timestamp)
            : this()
        {
            Timestamp = timestamp;
            ForDay = forDay;
            UserId = userId;
        }

        public const string ContentTemplate = "<IASPlatformUser Id=\"{0}\"/>";
        public const string TitleTemplate = "Calendar Changed for Day [{1}] of User [{0}]";
        public const string DateFormat = "yyyy-MM-dd";
        public virtual Guid Id { get; set; }

        public virtual string Title
        {
            get
            {
                if (string.IsNullOrEmpty(title))
                {
                    title = string.Format(TitleTemplate, UserId, ForDay.ToString(DateFormat));
                }
                return title;
            }
        }

        public virtual string Content
        {
            get
            {
                if (string.IsNullOrEmpty(content))
                {
                    content = String.Format(ContentTemplate, (object) UserId);
                }
                return content;
            }
        }

        public virtual long Timestamp { get; set; }

        public virtual DateTime LastUpdatedTime
        {
            get { return new DateTime(Timestamp, DateTimeKind.Utc); }
        }

        public virtual DateTime ForDay { get; protected set; }
        public virtual Guid UserId { get; protected set; }

        public virtual IList<FeedEntryLink> FeedEntryLinks
        {
            get { return new List<FeedEntryLink> { AlternativeLink, ForDayLink }; }
        }

        public virtual FeedEntryLink AlternativeLink
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

        public virtual FeedEntryLink ForDayLink
        {
            get
            {
                if (forDayLink == null)
                {
                    var uri = string.Format(FeedEntryLink.ForDayUrlTemplate, UserId, ForDay.ToString(DateFormat));
                    forDayLink = new FeedEntryLink(FeedEntryLink.ForDayRelationShip, uri);
                }
                return forDayLink;
            }
        }
    }

    public class FeedEntry : FeedEntryBase
    {
        public FeedEntry() : base()
        {
        }

        public FeedEntry(Guid userId, DateTime forDay, long timestamp)
            : base(userId, forDay, timestamp)
        {
        }
    }
}