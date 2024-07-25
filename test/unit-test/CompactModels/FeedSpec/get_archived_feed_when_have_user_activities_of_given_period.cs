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
    public class get_archived_feed_when_have_user_activities_of_given_period : CompactModelSpec
    {
        private static FeedController controller;
        private static readonly Guid UserId = Guid.NewGuid();
        private static readonly DateTime ForDay = new DateTime(2010, 5, 5);
        private static readonly DateTime LastUpdatedTimeInCurrent = new DateTime(2020, 1, 22);
        private static readonly DateTime LastUpdatedTimeInArchive = LastUpdatedTimeInCurrent.AddMonths(-2).AddDays(-1);

        [Fact]
        public void test_get_archived_feed_when_have_user_activities_of_given_period()
        {
            controller = new FeedController
            {
                Duration = 60,
                ControllerContext = new ControllerContext
                {
                    HttpContext = new HttpContextMock().SetupUrl("http://localhost")
                }
            };

            Session.Save(new UserActivityArchived(ForDay, UserId, LastUpdatedTimeInArchive.Ticks));
            Session.Save(new UserActivity(ForDay, UserId, LastUpdatedTimeInCurrent.Ticks));
            var result = controller.Archive(LastUpdatedTimeInArchive.Year,
                LastUpdatedTimeInArchive.Month,
                LastUpdatedTimeInArchive.Day,
                LastUpdatedTimeInArchive.Hour,
                LastUpdatedTimeInArchive.Minute);
            var feedResult = ((ContentResult)result).ReadAsSyndicationFeed();
            Assert.Single(feedResult.Items);
        }
    }
}