using System;
using System.Collections.Generic;
using System.Linq;
using Calendar.Client.Schema;
using Calendar.General.Persistence;
using Calendar.Model.Compact;
using Calendar.Model.Convertor;
using Xunit;

namespace Calendar.Tests.Unit.CompactModels.UserCalendarSpec
{
    [XUnitCases]
    public class when_retrieve_calendar_of_multiple_user_multiple_days : PersistenceSpec
    {
        private static readonly Guid User1 = Guid.NewGuid();
        private static readonly Guid User2 = Guid.NewGuid();
        private static readonly DateTime Day1 = new DateTime(2010, 5, 1);
        private static readonly DateTime Day2 = new DateTime(2010, 6, 1);
        private static CalendarRoot calendarRoot;
        private static UserCalendar calendarOfFirstUser;
        private static UserCalendar calendarOfSecondUser;

        private static readonly Dictionary<Guid, List<DateTime>> UserDays =
            new Dictionary<Guid, List<DateTime>>
            {
                { User1, new List<DateTime> { Day1, Day2 } },
                { User2, new List<DateTime> { Day1, Day2 } },
            };

        private void InitMonthActivity(Guid userId, DateTime date)
        {
            var monthActivity = new MonthActivity(userId, date.Year, date.Month);
            var activity = new DayActivityBuilder().FirstActivityType(ActivityType.Work).Day(date.Day).Build();
            monthActivity.Update(activity);
            session.SaveOrUpdateAndFlush(monthActivity);
        }

        private void cleanup_db()
        {
            Cleaner.CleanTable(DbHelper.GetSession(), typeof(MonthActivity));
        }

        [Fact]
        public void test_when_retrieve_calendar_of_multiple_user_multiple_days()
        {
            Given("context", () =>
            {
                session = DbHelper.GetSession();
                InitMonthActivity(User1, Day1);
                InitMonthActivity(User1, Day2);
                InitMonthActivity(User2, Day1);
                InitMonthActivity(User2, Day2);
            });
            When("of", () => { calendarRoot = new CalendarDtoRetriever(session).Retrieve(UserDays); });
            Then("should_return_data_of_all_users_and_all_days", () =>
            {
                Assert.Equal(2, calendarRoot.Users.Count);
                Assert.Contains(User1.ToString().ToUpper(), calendarRoot.Users.Select(user => user.IasPlatformUser.ToUpper()));
                Assert.Contains(User2.ToString().ToUpper(), calendarRoot.Users.Select(user => user.IasPlatformUser.ToUpper()));
                calendarOfFirstUser = calendarRoot.Users.First();
                calendarOfSecondUser = calendarRoot.Users.Last();
                Assert.Equal(2, calendarOfFirstUser.Days.Count);
                Assert.Equal(2, calendarOfSecondUser.Days.Count);
                Assert.Equal(Day1.ToString("yyyy-MM-dd"), calendarOfFirstUser.Days.First().Date);
                Assert.Equal(Day2.ToString("yyyy-MM-dd"), calendarOfFirstUser.Days.Last().Date);
                Assert.Equal(Day1.ToString("yyyy-MM-dd"), calendarOfSecondUser.Days.First().Date);
                Assert.Equal(Day2.ToString("yyyy-MM-dd"), calendarOfSecondUser.Days.Last().Date);
            });
        }
    }
}