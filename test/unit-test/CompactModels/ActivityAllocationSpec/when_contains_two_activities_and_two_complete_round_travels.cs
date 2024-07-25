using Calendar.Model.Compact;
using Calendar.Tests.Unit.Services.Fixtures;
using Xunit;

namespace Calendar.Tests.Unit.CompactModels.ActivityAllocationSpec
{
    [XUnitCases]
    public class when_contains_two_activities_and_two_complete_round_travels : DbSpec
    {
        private static DayActivity day;

        [Fact]
        public void test_when_contains_two_activities_and_two_complete_round_travels()
        {
            Given("context", () =>
            {
                var locationPattern = new LocationPattern(LocationValues.Beijing, null, 9, LocationValues.California, 19, 20, LocationValues.Beijing, 6);
                day = new DayActivityBuilder().FirstActivityType(ActivityType.National)
                    .SecondActivityType(ActivityType.Work)
                    .Location(locationPattern).Build();
            });
            Then("should_allocate_first_activity_to_arrival_location_of_first_travel", () =>
            {
                var allocationRule = new ActivityAllocationRule(day);
                Assert.Equal((byte)0, allocationRule.DefaultFirstActivityAllocation);
                Assert.Equal((byte)0, allocationRule.DefaultSecondActivityAllocation);
                Assert.False(allocationRule.IsFirstActivityFirstLocationAllocationEmpty());
                Assert.False(allocationRule.IsSecondActivityFirstLocationAllocationEmpty());

            });
        }
    }
}