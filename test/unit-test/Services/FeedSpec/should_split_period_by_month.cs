using System;
using Calendar.Models.Feed;
using Calendar.Persistence;
using Xunit;

namespace Calendar.Tests.Unit.Services.FeedSpec
{
    [XUnitCases]
    public class should_split_period_by_month : DbSpec
    {
        [Fact]
        public void test_should_split_period_by_month()
        {
            Then("when_period_at_same_month", () =>
            {
                var startTime = new DateTime(2012, 1, 1, 1, 0, 0, DateTimeKind.Utc);
                var endTime = new DateTime(2012, 1, 20, 1, 0, 0, DateTimeKind.Utc);
                MonthSplitter.FindDaysInPeriod(startTime, endTime, Guid.Empty, (s, e, userId) =>
                {
                    Assert.Equal(startTime, s);
                    Assert.Equal(endTime, e);
                    return Array.Empty<DateTime>();
                });
            });
            Then("when_period_at_cross_month", () =>
            {
                var startTime = new DateTime(2012, 1, 1, 1, 0, 0, DateTimeKind.Utc);
                var endTime = new DateTime(2012, 2, 20, 1, 0, 0, DateTimeKind.Utc);
                var index = 0;
                MonthSplitter.FindDaysInPeriod(startTime, endTime, Guid.Empty, (s, e, userId) =>
                {
                    if (index == 0)
                    {
                        Assert.Equal(startTime, s);
                        Assert.Equal(new DateTime(2012, 2, 1, 0, 0, 0, DateTimeKind.Utc), e);
                    }
                    else
                    {
                        Assert.Equal(new DateTime(2012, 2, 1, 0, 0, 0, DateTimeKind.Utc), s);
                        Assert.Equal(endTime, e);
                    }

                    index++;
                    return Array.Empty<DateTime>();
                });
                Assert.Equal(2, index);
            });
            Then("when_period_at_cross_month2", () =>
            {
                var startTime = new DateTime(2012, 1, 1, 1, 0, 0, DateTimeKind.Utc);
                var endTime = new DateTime(2012, 3, 1, 0, 0, 0, DateTimeKind.Utc);
                var period = new Period(startTime, endTime);
                var index = 0;
                MonthSplitter.FindDaysInPeriod(startTime, endTime, Guid.Empty, (s, e, userId) =>
                {
                    if (index == 0)
                    {
                        Assert.Equal(startTime, s);
                        Assert.Equal(new DateTime(2012, 2, 1, 0, 0, 0, DateTimeKind.Utc), e);
                    }
                    else
                    {
                        Assert.Equal(new DateTime(2012, 2, 1, 0, 0, 0, DateTimeKind.Utc), s);
                        Assert.Equal(endTime, e);
                    }

                    index++;
                    return Array.Empty<DateTime>();
                });
                Assert.Equal(2, index);
            });
        }
    }
}