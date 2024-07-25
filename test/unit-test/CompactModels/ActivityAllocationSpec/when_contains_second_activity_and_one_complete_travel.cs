using Calendar.Model.Compact;
using Calendar.Tests.Unit.Services.Fixtures;
using Xunit;

namespace Calendar.Tests.Unit.CompactModels.ActivityAllocationSpec
{
    [XUnitCases]
    public class when_contains_second_activity_and_one_complete_travel : DbSpec
    {
        private static DayActivity day;

        [Fact]
        public void test_when_contains_second_activity_and_one_complete_travel()
        {
            Given("context", () =>
            {
                var locationPattern = new LocationPattern(LocationValues.Beijing, null, 9, LocationValues.California, 19);
                day = new DayActivityBuilder().SecondActivityType(ActivityType.National)
                    .Location(locationPattern).Build();
            });
            Then("should_devide_second_activity_to_departure_and_arrival_locations", () =>
            {
                var allocationRule = new ActivityAllocationRule(day);
                Assert.Equal((byte)50, allocationRule.DefaultSecondActivityAllocation);
                Assert.True(allocationRule.IsFirstActivityFirstLocationAllocationEmpty());
            });
        }
    }
}