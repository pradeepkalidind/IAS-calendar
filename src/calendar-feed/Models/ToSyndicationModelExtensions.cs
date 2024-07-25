using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using Calendar.Models.Feed;
using NHibernate.Linq;

namespace Calendar.Feed.Models
{
    public static class ToSyndicationModelExtensions
    {
        public static SyndicationFeed ToSyndicationFeed(this IEnumerable<FeedEntryBase> entries, string host,
                                                        Uri prevArchiveUri, DateTime defaultUpdatedTime)
        {
            var items = entries.Select(feedEntry => feedEntry.ToSyndicationItem(host));
            var syndicationFeed = new SyndicationFeed(items);
            syndicationFeed.AddPreArchiveUri(prevArchiveUri);
            syndicationFeed.LastUpdatedTime = entries.Any()
                                                  ? entries.First().LastUpdatedTime
                                                  : defaultUpdatedTime.Date;
            return syndicationFeed;
        }

        private static void AddPreArchiveUri(this SyndicationFeed syndicationFeed, Uri prevArchiveUri)
        {
            if (prevArchiveUri != null)
            {
                var syndicationLink = new SyndicationLink(prevArchiveUri) { RelationshipType = "prev-archive" };
                syndicationFeed.Links.Add(syndicationLink);
            }
        }

        private static SyndicationItem ToSyndicationItem(this FeedEntryBase entry, string prefixUrl)
        {
            var item = new SyndicationItem
            {
                Title = new TextSyndicationContent(entry.Title),
                Content = new TextSyndicationContent(entry.Content),
                Id = entry.Id.ToString(),
                LastUpdatedTime = entry.LastUpdatedTime
            };
            entry.FeedEntryLinks.ToList().ForEach(link => item.Links.Add(link.ToSyndicationLink(prefixUrl)));
            return item;
        }

        private static SyndicationLink ToSyndicationLink(this FeedEntryLink link, string prefixUrl)
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