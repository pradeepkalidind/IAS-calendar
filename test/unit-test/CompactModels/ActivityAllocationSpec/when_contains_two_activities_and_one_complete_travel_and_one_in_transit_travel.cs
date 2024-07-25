using Calendar.Model.Compact;
using Calendar.Tests.Unit.Services.Fixtures;
using Xunit;

namespace Calendar.Tests.Unit.CompactModels.ActivityAllocationSpec
{
    [XUnitCases]
    public class when_contains_two_activities_and_one_complete_travel_and_one_in_transit_travel : DbSpec
    {
        private static DayActivity day;

        [Fact]
        public void test_when_contains_two_activities_and_one_complete_travel_and_one_in_transit_travel()
        {
            Given("context", () =>
            {
                var locationPattern = new LocationPattern(LocationValues.Beijing, null, 9, LocationValues.California, 19, null, LocationPattern.Intransit);
                day = new DayActivityBuilder().FirstActivityType(ActivityType.National)
                    .SecondActivityType(ActivityType.Work)
                    .Location(locationPattern).Build();
            });
            Then("should_allocate_first_activity_to_departure_location_and_second_activity_to_arrival_location_of_first_travel", () =>
            {
                var allocationRule = new ActivityAllocationRule(day);
                Assert.Equal((byte)100, allocationRule.DefaultFirstActivityAllocation);
                Assert.Equal((byte)0, allocationRule.DefaultSecondActivityAllocation);
                Assert.False(allocationRule.IsSecondActivityFirstLocationAllocationEmpty());
            });
        }
    }
}