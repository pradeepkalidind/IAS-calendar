using Calendar.Model.Compact;
using Calendar.Tests.Unit.Services.Fixtures;
using Xunit;

namespace Calendar.Tests.Unit.CompactModels.ActivityAllocationSpec
{
    [XUnitCases]
    public class when_contains_first_activity_and_one_location : DbSpec
    {
        private static DayActivity day;

        [Fact]
        public void test_when_contains_first_activity_and_one_location()
        {
            Given("context", () =>
            {
                var locationPattern = new LocationPattern(LocationValues.Beijing);
                day = new DayActivityBuilder()
                    .FirstActivityType(ActivityType.National)
                    .Location(locationPattern)
                    .Build();
            });
            Then("should_allocation_whole_activity_to_that_location", () =>
            {
                var allocationRule = new ActivityAllocationRule(day);
                Assert.Equal((byte)100, allocationRule.DefaultFirstActivityAllocation);
                Assert.True(allocationRule.IsSecondActivityFirstLocationAllocationEmpty());
            });
        }
    }
}