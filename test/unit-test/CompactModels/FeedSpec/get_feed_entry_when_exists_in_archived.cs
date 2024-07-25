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
    public class get_feed_entry_when_exists_in_archived : PersistenceSpec
    {
        private static FeedController controller;
        private static readonly Guid UserId = Guid.NewGuid();
        private static readonly DateTime ForDay = new DateTime(2010, 5, 5);
        private static readonly DateTime LastUpdatedTime = DateTime.UtcNow.AddMonths(-2).AddDays(-1);

        [Fact]
        public void test_get_feed_entry_when_exists_in_archived()
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
            When("of", () => session.Save(new FeedArchivedEntry(UserId, ForDay, LastUpdatedTime.Ticks)));
            var result = controller.Index();
            var feedResult = ((ContentResult)result).ReadAsSyndicationFeed();
            Assert.Single(feedResult.Items);
        }
    }
}