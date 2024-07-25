using System;
using Calendar.Client.Schema;
using Xunit;

namespace Calendar.Tests.Unit.Services.GeneralSpec.Models.FeedDaySpec
{
    [XUnitCases]
    public class when_compare_two_day_objects : DbSpec
    {
        private static Day first;
        private static Day second;
        private static readonly DateTime Date = DateTime.Now;

        [Fact]
        public void test_when_compare_two_day_objects()
        {
            Given("context", () =>
            {
                first = new Day
                {
                    Date = Date.ToString("yyyy-MM-dd"),
                    Location = "hello"
                };
                second = new Day
                {
                    Date = Date.ToString("yyyy-MM-dd"),
                    Location = "world"
                };
            });
            Then("should_equal_when_they_have_same_date", () => Assert.True(first.Equals(second)));
        }
    }
}