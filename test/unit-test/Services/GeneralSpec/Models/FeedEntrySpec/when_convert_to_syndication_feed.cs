using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using Calendar.Feed.Models;
using Calendar.Models.Feed;
using Xunit;

namespace Calendar.Tests.Unit.Services.GeneralSpec.Models.FeedEntrySpec
{
    [XUnitCases]
    public class when_convert_to_syndication_feed : DbSpec
    {
        private static readonly DateTime LastUpdatedTime = DateTime.UtcNow;
        private static readonly Guid UserId = Guid.NewGuid();
        private static readonly DateTime ForDay = new DateTime(2010, 5, 5);

        private static IList<FeedEntry> feedEntries;
        private static SyndicationItem item;

        [Fact]
        public void test_when_convert_to_syndication_feed()
        {
            Given("context", () => feedEntries = new List<FeedEntry>() { new FeedEntry(UserId, ForDay, LastUpdatedTime.Ticks) });
            When("of", () => item = feedEntries.ToSyndicationFeed("http://localhost", new Uri("http://localhost"), DateTime.Now).Items.First());
            Then("should_convert_feed_entry_to_feed_item", () =>
            {
                Assert.Equal(LastUpdatedTime, item.LastUpdatedTime);
                Assert.Equal(string.Format(FeedEntry.TitleTemplate, UserId, ForDay.ToString(FeedEntry.DateFormat)), item.Title.Text);
                Assert.Equal(string.Format(FeedEntry.ContentTemplate, feedEntries[0].UserId), ((TextSyndicationContent)item.Content).Text);
            });
            Then("should_append_prefix_to_feed_entry_link", () =>
            {
                Assert.Equal(2, item.Links.Count);
                var alternativeLink = item.Links.First();
                Assert.Equal(new Uri("http://localhost" + string.Format(FeedEntryLink.AlternativeUrlTemplate, UserId)), alternativeLink.Uri);
                var forDayLink = item.Links.Last();
                Assert.Equal(new Uri("http://localhost" + string.Format(FeedEntryLink.ForDayUrlTemplate, UserId, ForDay.ToString("yyyy-MM-dd"))), forDayLink.Uri);
            });
        }
    }
}