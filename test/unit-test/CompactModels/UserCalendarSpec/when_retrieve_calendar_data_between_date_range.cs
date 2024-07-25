using System;
using System.Collections.Generic;
using System.Linq;
using Calendar.Client.Schema;
using Calendar.General.Persistence;
using Calendar.Model.Compact;
using Calendar.Model.Convertor;
using Calendar.Persistence;
using Calendar.Tests.Unit.Services.Fixtures;
using Xunit;

namespace Calendar.Tests.Unit.CompactModels.UserCalendarSpec
{
    [XUnitCases]
    public class when_retrieve_calendar_data_between_date_range : DbSpec
    {
        private static ISessionWrapper session;
        private static Guid userId;
        private static UserCalendar userCalendar;

        private static void InitDayCalendar(MonthActivity monthActivity, DateTime baseDate)
        {
            var dayActivity = new DayActivity(new Model.Compact.Activity(ActivityType.Work, 20),
                new Model.Compact.Activity(ActivityType.Sick, 20),
                new LocationPattern(LocationValues.California, 2, 1,
                    "/Country/US", 14, 12,
                    "/Country/CA", 16), baseDate.Day, new List<string>());
            monthActivity.Update(dayActivity);
        }

        private void cleanup_db()
        {
            Cleaner.CleanTable(DbHelper.GetSession(), typeof(MonthActivity));
        }

        [Fact]
        public void test_when_retrieve_calendar_data_between_date_range()
        {
            Given("context", () =>
            {
                userId = Guid.NewGuid();
                session = DbHelper.GetSession();
                var monthActivity = new MonthActivity(userId, 2011, 5);
                InitDayCalendar(monthActivity, new DateTime(2011, 5, 5));
                InitDayCalendar(monthActivity, new DateTime(2011, 5, 7));
                InitDayCalendar(monthActivity, new DateTime(2011, 5, 9));
                session.SaveOrUpdateAndFlush(monthActivity);
            });
            When("of", () => userCalendar = new CalendarDtoRetriever(session).Retrieve(userId, new DateTime(2011, 5, 5), new DateTime(2011, 5, 8)));
            Then("should_return_calendar_with_day_in_range", () =>
            {
                Assert.Equal(2, userCalendar.Days.Count);
                Assert.Equal("2011-05-05", userCalendar.Days.First().Date);
                Assert.Equal("2011-05-07", userCalendar.Days.Last().Date);
            });
        }
    }
}