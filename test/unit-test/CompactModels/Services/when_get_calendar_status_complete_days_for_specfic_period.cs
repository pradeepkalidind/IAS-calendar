using System;
using System.Collections.Generic;
using Calendar.General.Persistence;
using Calendar.Model.Compact;
using Calendar.Persistence;
using Calendar.Service.Controller;
using Calendar.Tests.Unit.Services.Fixtures;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Calendar.Tests.Unit.CompactModels.Services
{
    [XUnitCases]
    public class when_get_calendar_status_complete_days_for_specfic_period : DbSpec
    {
        private static CalendarStatusController controller;
        private static readonly Guid UserId = Guid.NewGuid();
        private static ISessionWrapper session;

        private void cleanup_db()
        {
            Cleaner.CleanTable(DbHelper.GetSession(), typeof(MonthActivity));
        }

        [Fact]
        public void test_when_get_calendar_status_complete_days_for_specfic_period()
        {
            cleanup_db();
            Given("context", () =>
            {
                session = DbHelper.GetSession();
                controller = new CalendarStatusController();
                var monthActivity = new MonthActivity(UserId, 2010, 6);
                var locationPattern = new LocationPattern(LocationValues.NewYork);
                for (var i = 1; i <= 10; i++)
                {
                    var activity = new DayActivity(new Activity(ActivityType.Work, 0), new Activity(ActivityType.NonWork, 0), locationPattern, i, new List<string>());
                    monthActivity.Update(activity);
                    session.SaveOrUpdateAndFlush(monthActivity);
                }

                for (var i = 21; i <= 30; i++)
                {
                    var activity = new DayActivity(new Activity(ActivityType.Empty, 0), new Activity(ActivityType.Work, 0), locationPattern, i, new List<string>());
                    monthActivity.Update(activity);
                    session.SaveOrUpdateAndFlush(monthActivity);
                }

                var monthActivity2 = new MonthActivity(UserId, 2010, 7);
                for (var i = 1; i <= 10; i++)
                {
                    var activity = new DayActivity(new Activity(ActivityType.Work, 0), new Activity(ActivityType.Work, 0), locationPattern, i, new List<string>());
                    monthActivity2.Update(activity);
                    session.SaveOrUpdateAndFlush(monthActivity2);

                }
            });
            var result = controller.GetCompleteDays(UserId.ToString(), 2010, 6, 1, 2010, 7, 30);
            Assert.NotNull(result);
            Assert.Equal(20, ((OkObjectResult)result.Result)?.Value);
            var monthResult = controller.GetCompleteDaysByMonth(UserId.ToString(), 2010, 6, 1, 2010, 7, 30);
            Assert.Equal("10,10", ((ContentResult)monthResult).Content);
        }
    }
}