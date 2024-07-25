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
    public class when_get_calendar_status_complete : DbSpec
    {
        private static CalendarStatusController controller;
        private static readonly Guid userId = Guid.NewGuid();
        public static ISessionWrapper session;

        [Fact]
        public void test_when_get_calendar_status_complete()
        {
            Given("context", () =>
            {
                session = DbHelper.GetSession();
                controller = new CalendarStatusController();
                var monthActivity = new MonthActivity(userId, 2010, 6);

                for (int i = 1; i <= 30; i++)
                {
                    var locationPattern = new LocationPattern(LocationValues.NewYork);
                    var dayActivity = new DayActivity(new Activity(ActivityType.Work, 20),
                        new Activity(ActivityType.Sick, 0), locationPattern, i, new List<string>());
                    monthActivity.Update(dayActivity);
                }

                session.SaveOrUpdateAndFlush(monthActivity);
            });
            var result = controller.IsCompleted(userId.ToString(), 2010, 6, 2010, 6);
            Assert.NotNull(result);
            Assert.Equal(true, ((OkObjectResult)result.Result)?.Value);
            var inCompleteResult = controller.IsCompleted(userId.ToString(), 2010, 6, 2010, 7);
            Assert.NotNull(inCompleteResult);
            Assert.Equal(false, ((OkObjectResult)inCompleteResult.Result)?.Value);
        }
    }
}