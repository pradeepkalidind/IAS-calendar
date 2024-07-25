using System;
using Calendar.Feed.Services;
using Calendar.Models.Feed;
using Xunit;

namespace Calendar.Tests.Unit.Services.FeedSpec
{
    [XUnitCases]
    public class when_calculate_period : DbSpec
    {
        private static int duration;
        private static Period period;
        private static DateTime time;

        [Fact]
        public void test_when_calculate_period()
        {
            Then("should_succeed_time1", () =>
            {
                time = new DateTime(2011, 8, 23, 3, 58, 0, DateTimeKind.Utc);
                duration = 30;
                period = PeriodCalculator.Calculate(time, duration);
                Assert.Equal(new DateTime(2011, 8, 23, 3, 30, 0, DateTimeKind.Utc), period.StartTime);
                Assert.Equal(new DateTime(2011, 8, 23, 4, 0, 0, DateTimeKind.Utc), period.EndTime);
            });
            Then("should_succeed_time2", () =>
            {
                time = new DateTime(2011, 8, 23, 4, 0, 0, DateTimeKind.Utc);
                duration = 30;
                period = PeriodCalculator.Calculate(time, duration);
                Assert.Equal(new DateTime(2011, 8, 23, 4, 0, 0, DateTimeKind.Utc), period.StartTime);
                Assert.Equal(new DateTime(2011, 8, 23, 4, 30, 0, DateTimeKind.Utc), period.EndTime);
            });
            Then("should_succeed_time3", () =>
            {
                time = new DateTime(2011, 8, 23, 3, 29, 0, DateTimeKind.Utc);
                duration = 10;
                period = PeriodCalculator.Calculate(time, duration);
                Assert.Equal(new DateTime(2011, 8, 23, 3, 20, 0, DateTimeKind.Utc), period.StartTime);
                Assert.Equal(new DateTime(2011, 8, 23, 3, 30, 0, DateTimeKind.Utc), period.EndTime);
            });
            Then("should_succeed_time4", () =>
            {
                time = new DateTime(2011, 8, 23, 3, 20, 0, DateTimeKind.Utc);
                duration = 10;
                period = PeriodCalculator.Calculate(time, duration);
                Assert.Equal(new DateTime(2011, 8, 23, 3, 20, 0, DateTimeKind.Utc), period.StartTime);
                Assert.Equal(new DateTime(2011, 8, 23, 3, 30, 0, DateTimeKind.Utc), period.EndTime);
            });
            Then("should_succeed_time5", () =>
            {
                time = new DateTime(2011, 8, 23, 3, 20, 0, DateTimeKind.Utc);
                duration = 60;
                period = PeriodCalculator.Calculate(time, duration);
                Assert.Equal(new DateTime(2011, 8, 23, 3, 0, 0, DateTimeKind.Utc), period.StartTime);
                Assert.Equal(new DateTime(2011, 8, 23, 4, 0, 0, DateTimeKind.Utc), period.EndTime);
            });
            Then("should_succeed_time6", () =>
            {
                time = new DateTime(2011, 8, 23, 3, 0, 0, DateTimeKind.Utc);
                duration = 60;
                period = PeriodCalculator.Calculate(time, duration);
                Assert.Equal(new DateTime(2011, 8, 23, 3, 0, 0, DateTimeKind.Utc), period.StartTime);
                Assert.Equal(new DateTime(2011, 8, 23, 4, 0, 0, DateTimeKind.Utc), period.EndTime);
            });
            Then("should_succeed_time7", () =>
            {
                time = new DateTime(2011, 8, 23, 7, 0, 0, DateTimeKind.Utc);
                duration = 240;
                period = PeriodCalculator.Calculate(time, duration);
                Assert.Equal(new DateTime(2011, 8, 23, 4, 0, 0, DateTimeKind.Utc), period.StartTime);
                Assert.Equal(new DateTime(2011, 8, 23, 8, 0, 0, DateTimeKind.Utc), period.EndTime);
            });
            Then("should_succeed_time8", () =>
            {
                time = new DateTime(2011, 8, 23, 8, 0, 0, DateTimeKind.Utc);
                duration = 240;
                period = PeriodCalculator.Calculate(time, duration);
                Assert.Equal(new DateTime(2011, 8, 23, 8, 0, 0, DateTimeKind.Utc), period.StartTime);
                Assert.Equal(new DateTime(2011, 8, 23, 12, 0, 0, DateTimeKind.Utc), period.EndTime);
            });
            Then("should_succeed_time9", () =>
            {
                time = new DateTime(2011, 8, 23, 7, 0, 0, DateTimeKind.Utc);
                duration = 1440;
                period = PeriodCalculator.Calculate(time, duration);
                Assert.Equal(new DateTime(2011, 8, 23, 0, 0, 0, DateTimeKind.Utc), period.StartTime);
                Assert.Equal(new DateTime(2011, 8, 24, 0, 0, 0, DateTimeKind.Utc), period.EndTime);
            });
            Then("should_succeed_time10", () =>
            {
                time = new DateTime(2011, 8, 23, 0, 0, 0, DateTimeKind.Utc);
                duration = 1440;
                period = PeriodCalculator.Calculate(time, duration);
                Assert.Equal(new DateTime(2011, 8, 23, 0, 0, 0, DateTimeKind.Utc), period.StartTime);
                Assert.Equal(new DateTime(2011, 8, 24, 0, 0, 0, DateTimeKind.Utc), period.EndTime);
            });
            Then("should_succeed_time11", () =>
            {
                time = new DateTime(2011, 8, 4, 9, 20, 7, 297, DateTimeKind.Utc);
                duration = 60;
                period = PeriodCalculator.Calculate(time, duration);
                Assert.Equal(new DateTime(2011, 8, 4, 9, 0, 0, DateTimeKind.Utc), period.StartTime);
                Assert.Equal(new DateTime(2011, 8, 4, 10, 0, 0, DateTimeKind.Utc), period.EndTime);
            });
        }
    }
}