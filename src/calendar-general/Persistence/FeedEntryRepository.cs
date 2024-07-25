using System;
using System.Collections.Generic;
using System.Linq;
using Calendar.Models.Feed;
using Calendar.Persistence;

namespace Calendar.General.Persistence
{
    public class FeedEntryRepository
    {
        private readonly ISessionWrapper session;

        public FeedEntryRepository(ISessionWrapper session)
        {
            this.session = session;
        }

        public IEnumerable<FeedEntryBase> FindEntriesInPeriod(Period period)
        {
            var startTimestamp = period.StartTime.Ticks;
            var finishTimestamp = period.EndTime.Ticks;

            var feedEntries = FindEntriesInPeriod<FeedEntry>(startTimestamp, finishTimestamp);
            if (feedEntries.Count() < 1)
            {
                return FindEntriesInPeriod<FeedArchivedEntry>(startTimestamp, finishTimestamp).ToArray();
            }
            return feedEntries.ToArray();
        }

        public IEnumerable<DateTime> FindDaysInPeriod(Guid userId, Period period)
        {
            return MonthSplitter.FindDaysInPeriod(period.StartTime, period.EndTime, userId, GetDaysByMonth);
        }

        private IEnumerable<DateTime> GetDaysByMonth(DateTime start, DateTime end, Guid userId)
        {
            var startTimestamp = start.Ticks;
            var finishTimestamp = end.Ticks;

            var daysInPeriod = FindEntriesInPeriod<FeedEntry>(startTimestamp, finishTimestamp)
                .Where(f => f.UserId == userId)
                .Select(f => f.ForDay).ToArray();

            var archivedDays = FindEntriesInPeriod<FeedArchivedEntry>(startTimestamp, finishTimestamp)
                .Where(f => f.UserId == userId)
                .Select(f => f.ForDay).ToArray();
            return daysInPeriod.Concat(archivedDays);
        }

        public FeedEntryBase FindMostRecentBefore(DateTime dateTime)
        {
            var finishTimestamp = dateTime.ToUniversalTime().Ticks;
            var startTimestamp = dateTime.AddMonths(-1).ToUniversalTime().Ticks;
            var feedEntry = session.Query<FeedEntry>()
                .Where(f => f.Timestamp < finishTimestamp && f.Timestamp > startTimestamp)
                .OrderByDescending(f => f.Timestamp).FirstOrDefault();
            return feedEntry ?? FindMostRecentBeforeFromArchived(finishTimestamp);
        }

        private FeedEntryBase FindMostRecentBeforeFromArchived(long finishTimestamp)
        {
            return session.Query<FeedArchivedEntry>()
                .Where(f => f.Timestamp < finishTimestamp)
                .OrderByDescending(f => f.Timestamp).FirstOrDefault();
        }

        private IQueryable<T> FindEntriesInPeriod<T>(long startTimestamp, long finishTimestamp)
            where T : FeedEntryBase
        {
            return session.Query<T>()
                .Where(f => f.Timestamp >= startTimestamp && f.Timestamp < finishTimestamp)
                .OrderByDescending(f => f.Timestamp);
        }
    }
}