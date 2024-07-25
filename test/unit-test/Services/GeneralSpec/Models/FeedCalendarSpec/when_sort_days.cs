using System;
using System.Linq;
using Calendar.Client.Schema;
using Xunit;

namespace Calendar.Tests.Unit.Services.GeneralSpec.Models.FeedCalendarSpec
{
    [XUnitCases]
    public class when_sort_days : DbSpec
    {
        private static UserCalendar userCalendar;

        [Fact]
        public void test_when_sort_days()
        {
            Given("context", () =>
            {
                userCalendar = new UserCalendar("");
                userCalendar.FindOrCreate(new DateTime(2011, 11, 3));
                userCalendar.FindOrCreate(new DateTime(2011, 3, 12));
                userCalendar.FindOrCreate(new DateTime(2011, 3, 5));
                userCalendar.FindOrCreate(new DateTime(2011, 1, 8));
            });
            When("of", () => userCalendar.SortDays());
            Then("should_sort_by_date_of_days_in_asdent_sequence", () =>
            {
                Assert.Equal(4, userCalendar.Days.Count);
                Assert.Equal("2011-01-08", userCalendar.Days.ElementAt(0).Date);
                Assert.Equal("2011-03-05", userCalendar.Days.ElementAt(1).Date);
                Assert.Equal("2011-03-12", userCalendar.Days.ElementAt(2).Date);
                Assert.Equal("2011-11-03", userCalendar.Days.ElementAt(3).Date);
            });
        }
    }
}