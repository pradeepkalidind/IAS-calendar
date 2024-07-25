using System;
using Calendar.Model.Compact;
using Calendar.Service.Controller;
using Calendar.Tests.Unit.CompactModels.LocationRulesSpec;
using HttpContextMoq;
using HttpContextMoq.Extensions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Calendar.Tests.Unit.CompactModels.FeedSpec
{
    [XUnitCases]
    public class get_feed_entry_when_exists_user_activities_in_archived : CompactModelSpec
    {
        private static FeedController controller;
        private static readonly Guid UserId = Guid.NewGuid();
        private static readonly DateTime ForDay = new DateTime(2010, 5, 5);
        private static readonly DateTime LastUpdatedTime = new DateTime(2020, 1, 22).AddMonths(-2).AddDays(-1);

        [Fact]
        public void test_get_feed_entry_when_exists_user_activities_in_archived()
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
            When("of", () => Session.Save(new UserActivityArchived(ForDay, UserId, LastUpdatedTime.Ticks)));

            var result = controller.Index();
            var feedResult = ((ContentResult)result).ReadAsSyndicationFeed();
            var feedItems = feedResult.Items;
            Assert.Single(feedItems);
        }
    }
}