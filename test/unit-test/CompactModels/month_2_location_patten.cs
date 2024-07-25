using System;
using System.Collections.Generic;
using System.Linq;
using Calendar.Model.Compact;
using Calendar.Tests.Unit.CompactModels.LocationRulesSpec;
using Xunit;

namespace Calendar.Tests.Unit.CompactModels
{
    [XUnitCases]
    public class month_2_location_patten : CompactModelSpec
    {
        [Fact]
        public void test_month_2_location_patten()
        {
            Then("should_create_empty_by_default", () =>
            {
                var monthActivity = new MonthActivity(Guid.Empty, 2012, 1);
                var dayActivity = monthActivity.GetDays().First();
                Assert.Equal(LocationPattern.Empty(), dayActivity.LocationPattern);
            });
            Then("should_update", () =>
            {
                var monthActivity = new MonthActivity(Guid.Empty, 2012, 1);

                var inUS = new LocationPattern("US");

                var activity = new Activity(ActivityType.Work, 100);
                var workAllDayInUs = new DayActivity(activity, activity, inUS, 1, new List<string>());
                monthActivity.Update(workAllDayInUs);
                Assert.Equal(inUS, monthActivity.GetDays().First().LocationPattern);
            });
        }
    }
}