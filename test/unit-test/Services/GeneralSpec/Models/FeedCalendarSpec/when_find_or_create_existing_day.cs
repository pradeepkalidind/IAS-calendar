using System;
using System.Collections.Generic;
using Calendar.Client.Schema;
using Xunit;

namespace Calendar.Tests.Unit.Services.GeneralSpec.Models.FeedCalendarSpec
{
    [XUnitCases]
    public class when_find_or_create_existing_day : DbSpec
    {
        private static UserCalendar userCalendar;
        private static readonly DateTime Date = new DateTime(2011, 5, 5);
        private static Day day;

        [Fact]
        public void test_when_find_or_create_existing_day()
        {
            Given("context", () =>
            {
                userCalendar = new UserCalendar("")
                {
                    Days = new HashSet<Day>
                    {
                        new Day
                        {
                            Date = "2011-05-05"
                        }
                    }
                };
            });
            When("of", () => day = userCalendar.FindOrCreate(Date));
            Then("should_find_existing_day_in_calendar", () =>
            {
                Assert.Single(userCalendar.Days);
                Assert.Equal("2011-05-05", day.Date);
            });
        }
    }
}