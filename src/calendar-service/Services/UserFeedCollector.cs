using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using Calendar.General.Persistence;
using Calendar.Model.Compact;
using Calendar.Models.Feed;
using Calendar.Persistence;

namespace Calendar.Service.Services
{
    public class UserFeedCollector
    {
        private readonly Period currentPeriod;
        private readonly ISessionWrapper session;
        private readonly FeedEntryRepository feedEntryRepo;
        public IList<UserFeed> userFeeds = new List<UserFeed>();

        public UserFeedCollector(Period currentPeriod,ISessionWrapper session,FeedEntryRepository feedEntryRepo)
        {
            this.currentPeriod = currentPeriod;
            this.session = session;
            this.feedEntryRepo = feedEntryRepo;
        }

        public SyndicationFeed ToSyndicationFeed(string host, Uri prevArchiveUri, DateTime defaultUpdatedTime)
        {
            BuildUserFeeds();

            var orderedFeeds = userFeeds.OrderByDescending(u=>u.LastUpdatedTime);
            var items = orderedFeeds
                .Select(userFeed => userFeed.ToSyndicationItem(host));
            var syndicationFeed = new SyndicationFeed(items);
            AddPreArchiveUri(syndicationFeed,prevArchiveUri);
            syndicationFeed.LastUpdatedTime = orderedFeeds.Count() > 0
                                                  ? orderedFeeds.First().LastUpdatedTime
                                                  : defaultUpdatedTime.Date;
            return syndicationFeed;
        }

        public IEnumerable<DateTime> GetChangedDays(Guid userId)
        {
            var days = feedEntryRepo.FindDaysInPeriod(userId, currentPeriod);
            var activitiesInPeriod = new Repository(session).FindUserActivitiesInPeriod(userId,currentPeriod.StartTime,currentPeriod.EndTime);
            return activitiesInPeriod.Concat(days).Distinct();
        }
            
        private void BuildUserFeeds()
        {
            feedEntryRepo.FindEntriesInPeriod(currentPeriod).ToList().ForEach(feedEntry => Add(feedEntry.Id, feedEntry.UserId, feedEntry.ForDay, feedEntry.Timestamp));
            var startTimestamp = currentPeriod.StartTime.Ticks;
            var finishTimestamp = currentPeriod.EndTime.Ticks;
            new Repository(session).FindUserActivitiesInPeriod(startTimestamp, finishTimestamp).ToList().ForEach(userActivity => Add(userActivity.Id, userActivity.UserId, userActivity.Date, userActivity.Timestamp));
        }

        private void Add(Guid id, Guid userId, DateTime date, long timestamp)
        {
            var userFeed = userFeeds.FirstOrDefault(u=>u.UserId.Equals(userId));
            if (userFeed == null)
            {
                userFeed = new UserFeed(id, userId, currentPeriod.StartTime, currentPeriod.EndTime);
                userFeeds.Add(userFeed);
            }
            userFeed.AddDay(date, timestamp);
        }

        private static void AddPreArchiveUri(SyndicationFeed syndicationFeed, Uri prevArchiveUri)
        {
            if (prevArchiveUri != null)
            {
                var syndicationLink = new SyndicationLink(prevArchiveUri) { RelationshipType = "prev-archive" };
                syndicationFeed.Links.Add(syndicationLink);
            }
        }
    }
}