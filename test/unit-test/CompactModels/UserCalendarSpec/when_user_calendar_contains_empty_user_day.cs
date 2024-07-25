using System;
using Calendar.Model.Convertor;
using Calendar.Persistence;
using Xunit;

namespace Calendar.Tests.Unit.CompactModels.UserCalendarSpec
{
    [XUnitCases]
    public class when_user_calendar_contains_empty_user_day : DbSpec
    {
        private static Guid userId;
        private static ISessionWrapper session;

        [Fact]
        public void test_when_user_calendar_contains_empty_user_day()
        {
            Given("context", () =>
            {
                userId = Guid.NewGuid();
                session = DbHelper.GetSession();
            });
            Then("CalendarBetween_should_not_contain_day_dto_of_empty_user_day", () => Assert.Empty(new CalendarDtoRetriever(session)
                .Retrieve(userId, DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1))
                .Days));
            Then("CalendarOf_should_not_contain_day_dto_of_empty_user_day", () => Assert.Empty(new CalendarDtoRetriever(session).Retrieve(userId).Days));
        }
    }
}