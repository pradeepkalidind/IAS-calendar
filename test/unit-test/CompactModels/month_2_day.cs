using System;
using System.Collections.Generic;
using System.Linq;
using Calendar.Model.Compact;
using Xunit;

namespace Calendar.Tests.Unit.CompactModels
{
    [XUnitCases]
    public class month_2_day : DbSpec
    {
        private static DayActivity CreateStandard(ActivityType type, int day)
        {
            var activity = new Activity(type, FULL);
            return new DayActivity(activity, activity, LocationPattern.Empty(), day, new List<string>());
        }

        private static DayActivity CreateWithEmptyLocation(ActivityType firstType, ActivityType secondType, int day)
        {
            return new DayActivity(new Activity(firstType, FULL), new Activity(secondType, FULL), LocationPattern.Empty(), day, new List<string>());
        }

        private const byte FULL = 100;

        [Fact]
        public void test_month_2_day()
        {
            Then("should_create_empty_by_default", () =>
            {
                var monthActivity = new MonthActivity(Guid.Empty, 2012, 1);
                var dayActivity = monthActivity.GetDays().First();
                Assert.Equal(CreateStandard(ActivityType.Empty, 1), dayActivity);
            });
            Then("should_update", () =>
            {
                var monthActivity = new MonthActivity(Guid.Empty, 2012, 1);
                var workAllDay = CreateStandard(ActivityType.Work, 1);
                monthActivity.Update((DayActivity)workAllDay);
                Assert.Equal(workAllDay, monthActivity.GetDays().First());
                Assert.Equal(1, monthActivity.GetDays().First().Day);

                var halfWorkDay = CreateWithEmptyLocation(ActivityType.Work, ActivityType.NonWork, 2);
                monthActivity.Update((DayActivity)halfWorkDay);
                Assert.Equal(halfWorkDay, monthActivity.GetDays().ElementAt(1));

                var firstActivityDay = CreateWithEmptyLocation(ActivityType.Work, ActivityType.Empty, 3);
                monthActivity.Update((DayActivity)firstActivityDay);
                var dayActivity = monthActivity.GetDays().ElementAt(2);
                Assert.Equal(firstActivityDay, dayActivity);
                Assert.Equal((byte)100, dayActivity.First.FirstLocationAllocation);

                var secondActivityDay = CreateWithEmptyLocation(ActivityType.Empty, ActivityType.Work, 4);
                monthActivity.Update((DayActivity)secondActivityDay);
                var dayActivity2 = monthActivity.GetDays().ElementAt(3);
                Assert.Equal(secondActivityDay, dayActivity2);
                Assert.Equal((byte)100, dayActivity2.Second.FirstLocationAllocation);
            });
        }
    }
}