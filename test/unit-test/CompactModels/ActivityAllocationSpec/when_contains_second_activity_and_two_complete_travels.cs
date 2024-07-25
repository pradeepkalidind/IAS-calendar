using Calendar.Model.Compact;
using Calendar.Tests.Unit.Services.Fixtures;
using Xunit;

namespace Calendar.Tests.Unit.CompactModels.ActivityAllocationSpec
{
    [XUnitCases]
    public class when_contains_second_activity_and_two_complete_travels : DbSpec
    {
        private static DayActivity day;

        [Fact]
        public void test_when_contains_second_activity_and_two_complete_travels()
        {
            Given("context", () =>
            {
                var locationPattern = new LocationPattern(LocationValues.Beijing, null, 9, LocationValues.California, 19, 20, LocationValues.NewYork, 6);
                day = new DayActivityBuilder().SecondActivityType(ActivityType.National)
                    .Location(locationPattern).Build();
            });
            Then("should_allocate_second_activity_to_arrival_location_of_first_travel", () =>
            {
                var allocationRule = new ActivityAllocationRule(day);
                Assert.True(allocationRule.IsFirstActivityFirstLocationAllocationEmpty());
                Assert.Equal((byte)50, allocationRule.DefaultSecondActivityAllocation);
            });
        }
    }
}