using System;
using Calendar.Models.Feed;
using Calendar.Service.Controller;
using HttpContextMoq;
using HttpContextMoq.Extensions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Calendar.Tests.Unit.CompactModels.FeedSpec
{
    [XUnitCases]
    public class get_archived_feed_entries_of_given_period : PersistenceSpec
    {
        private static FeedController controller;
        private static readonly Guid UserId = Guid.NewGuid();
        private static readonly DateTime ForDay = new DateTime(2010, 5, 5);
        private static readonly DateTime LastUpdatedTimeInCurrent = DateTime.UtcNow;
        private static readonly DateTime LastUpdatedTimeInArchive = LastUpdatedTimeInCurrent.AddMonths(-2).AddDays(-1);

        [Fact]
        public void test_get_archived_feed_entries_of_given_period()
        {
            Given("context", () =>
            {
                controller = new FeedController
                {
                    Duration = 60,
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new HttpContextMock().SetupUrl("http://localhost")
                    }
                };
            });

            When("of", () =>
            {
                session.Save(new FeedArchivedEntry(UserId, ForDay, LastUpdatedTimeInArchive.Ticks));
                session.Save(new FeedEntry(UserId, ForDay, LastUpdatedTimeInCurrent.Ticks));
            });

            var result = controller.Archive(
                LastUpdatedTimeInArchive.Year,
                LastUpdatedTimeInArchive.Month,
                LastUpdatedTimeInArchive.Day,
                LastUpdatedTimeInArchive.Hour,
                LastUpdatedTimeInArchive.Minute);

            var feedResult = ((ContentResult)result).ReadAsSyndicationFeed();
            Assert.Single(feedResult.Items);
        }
    }
}