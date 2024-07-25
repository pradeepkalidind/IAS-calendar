using Calendar.Model.Compact;
using Calendar.Tests.Unit.Services.Fixtures;
using Xunit;

namespace Calendar.Tests.Unit.CompactModels.ActivityAllocationSpec
{
    [XUnitCases]
    public class when_contains_two_activities_and_one_in_transit_travel : DbSpec
    {
        private static DayActivity day;

        [Fact]
        public void test_when_contains_two_activities_and_one_in_transit_travel()
        {
            Given("context", () =>
            {
                var locationPattern = new LocationPattern(LocationValues.Beijing, null, 9, LocationPattern.Intransit, 19);
                day = new DayActivityBuilder().FirstActivityType(ActivityType.National)
                    .SecondActivityType(ActivityType.Work)
                    .Location(locationPattern).Build();
            });
            Then("should_allocate_both_activities_to_departure_location", () =>
            {
                var allocationRule = new ActivityAllocationRule(day);
                Assert.Equal((byte)100, allocationRule.DefaultFirstActivityAllocation);
                Assert.Equal((byte)100, allocationRule.DefaultSecondActivityAllocation);

            });
        }
    }
}