using System.Collections.Generic;
using Calendar.Model.Compact;
using Calendar.Tests.Unit.Services.Fixtures;
using Xunit;

namespace Calendar.Tests.Unit.CompactModels.ActivityAllocationSpec
{
    [XUnitCases]
    public class when_contains_two_activities_and_one_location : DbSpec
    {

        private static DayActivity day;

        [Fact]
        public void test_when_contains_two_activities_and_one_location()
        {
            Given("context",
                () =>
                {
                    day = new DayActivity(new Activity(ActivityType.National, 0), new Activity(ActivityType.Work, 0), new LocationPattern(LocationValues.Beijing), 1,
                        new List<string>());
                });
            Then("should_allocation_both_activities_to_that_location", () =>
            {
                var allocationRule = new ActivityAllocationRule(day);
                Assert.Equal((byte)100, allocationRule.DefaultFirstActivityAllocation);
                Assert.Equal((byte)100, allocationRule.DefaultSecondActivityAllocation);
            });
        }
    }
}