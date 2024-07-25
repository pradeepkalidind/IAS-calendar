using System;
using System.Collections.Generic;
using System.Linq;
using Calendar.General.Persistence;
using Calendar.Models.Feed;
using Xunit;

namespace Calendar.Tests.Unit.Services.GeneralSpec.Models.FeedEntryRepositorySpec
{
    [XUnitCases]
    public class when_find_feed_entries_by_given_hour : PersistenceSpec
    {
        private static IEnumerable<FeedEntryBase> entries;
        private static FeedEntry feed1;
        private static FeedEntry feed2;
        private static FeedEntry feed3;

        private FeedEntry CreateFeedEntry(int hour, int minute)
        {
            var feedEntry1 = new FeedEntry(Guid.NewGuid(), new DateTime(2011, 1, 2),
                new DateTime(2011, 8, 8, hour, minute, 0, DateTimeKind.Utc).Ticks);

            session.CreateAndFlush(feedEntry1);
            return feedEntry1;
        }

        public when_find_feed_entries_by_given_hour()
        {
            feed1 = CreateFeedEntry(12, 10);
            feed2 = CreateFeedEntry(12, 15);
            feed3 = CreateFeedEntry(13, 10);
        }

        [Fact]
        public void test_when_find_feed_entries_by_given_hour()
        {
            When("of", () =>
            {
                var startTime = new DateTime(2011, 8, 8, 12, 0, 0);
                entries = new FeedEntryRepository(session).FindEntriesInPeriod(new Period(startTime, startTime.AddHours(1)));
            });
            Then("should_not_return_entry_not_of_given_hour", () => Assert.Null(entries.Where(e => e.UserId.Equals(feed3.UserId)).FirstOrDefault()));
            Then("should_return_all_entries_of_hour", () => Assert.Equal(2, entries.Count()));
            Then("should_sort_entries_in_descend_order", () =>
            {
                Assert.Equal(feed2.UserId, entries.First().UserId);
                Assert.Equal(feed2.ForDay, entries.First().ForDay);
                Assert.Equal(feed1.UserId, entries.Last().UserId);
                Assert.Equal(feed1.ForDay, entries.Last().ForDay);
            });
        }
    }
}