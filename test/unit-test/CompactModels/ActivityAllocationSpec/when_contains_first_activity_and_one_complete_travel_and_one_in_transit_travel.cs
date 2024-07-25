using Calendar.Model.Compact;
using Calendar.Tests.Unit.Services.Fixtures;
using Xunit;

namespace Calendar.Tests.Unit.CompactModels.ActivityAllocationSpec
{
    [XUnitCases]
    public class when_contains_first_activity_and_one_complete_travel_and_one_in_transit_travel : DbSpec
    {
        private static DayActivity day;

        [Fact]
        public void test_when_contains_first_activity_and_one_complete_travel_and_one_in_transit_travel()
        {
            Given("context", () =>
            {
                var locationPattern = new LocationPattern(LocationValues.Beijing, null, 9, LocationValues.California, 19, null, LocationPattern.Intransit);
                day = new DayActivityBuilder()
                    .FirstActivityType(ActivityType.National)
                    .Location(locationPattern)
                    .Build();
            });
            Then("should_devide_first_activity_to_departure_and_arrival_location_of_first_travel", () =>
            {
                var allocationRule = new ActivityAllocationRule(day);
                Assert.Equal((byte)50, allocationRule.DefaultFirstActivityAllocation);
                Assert.True(allocationRule.IsSecondActivityFirstLocationAllocationEmpty());
            });
        }
    }
}