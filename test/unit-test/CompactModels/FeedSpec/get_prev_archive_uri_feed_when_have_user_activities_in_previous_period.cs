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
    public class get_prev_archive_uri_feed_when_have_user_activities_in_previous_period : CompactModelSpec
    {
        private static FeedController controller;
        private static readonly Guid UserId = Guid.NewGuid();
        private static readonly DateTime ForDay = new DateTime(2010, 5, 5);
        private static readonly DateTime LastUpdatedTimeInCurrent = DateTime.UtcNow;
        private static readonly DateTime OneDayBefore = LastUpdatedTimeInCurrent.AddDays(-1);

        [Fact]
        public void test_get_prev_archive_uri_feed_when_have_user_activities_in_previous_period()
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
                Session.Save(new UserActivity(ForDay, UserId, OneDayBefore.Ticks));
                Session.Save(new UserActivity(ForDay, UserId, LastUpdatedTimeInCurrent.Ticks));
            });

            var result = controller.Archive(LastUpdatedTimeInCurrent.Year,
                LastUpdatedTimeInCurrent.Month,
                LastUpdatedTimeInCurrent.Day,
                LastUpdatedTimeInCurrent.Hour,
                LastUpdatedTimeInCurrent.Minute);
            var feedResult = ((ContentResult)result).ReadAsSyndicationFeed();
            var links = feedResult.Links;
            Assert.Single(links);
            var syndicationLink = links[0];
            var expected = $"http://localhost/{OneDayBefore.Year}/{OneDayBefore.Month}/{OneDayBefore.Day}/{OneDayBefore.Hour}/{0}/CalendarChanged.atom";
            Assert.Equal(expected, syndicationLink.Uri.ToString());
        }
    }
}