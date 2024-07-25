using Calendar.Model.Compact;
using Xunit;

namespace Calendar.Tests.Unit.CompactModels.ActivityAllocationSpec
{
    [XUnitCases]
    public class when_contains_no_location : DbSpec
    {
        private static DayActivity day;

        [Fact]
        public void test_when_contains_no_location()
        {
            Given("context", () =>
            {
                day = new DayActivityBuilder().FirstActivityType(ActivityType.National)
                    .SecondActivityType(ActivityType.Work)
                    .Build();
            });
            Then("should_use_default_allocation_rule", () =>
            {
                var allocationRule = new ActivityAllocationRule(day);
                Assert.True(allocationRule.IsFirstActivityFirstLocationAllocationEmpty());
                Assert.True(allocationRule.IsSecondActivityFirstLocationAllocationEmpty());
            });
        }
    }
}