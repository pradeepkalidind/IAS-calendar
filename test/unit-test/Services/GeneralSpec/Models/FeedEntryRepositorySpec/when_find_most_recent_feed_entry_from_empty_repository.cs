using System;
using Calendar.General.Persistence;
using Calendar.Models.Feed;
using Xunit;

namespace Calendar.Tests.Unit.Services.GeneralSpec.Models.FeedEntryRepositorySpec
{
    [XUnitCases]
    public class when_find_most_recent_feed_entry_from_empty_repository : PersistenceSpec
    {
        private static FeedEntryBase entry;

        [Fact]
        public void test_when_find_most_recent_feed_entry_from_empty_repository()
        {
            When("of", () => entry = new FeedEntryRepository(session).FindMostRecentBefore(DateTime.UtcNow.Date.AddDays(1)));
            Then("should_return_null", () => Assert.Null(entry));
        }
    }
}