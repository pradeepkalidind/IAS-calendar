using System.Collections.Generic;
using Calendar.Model.Compact;
using Calendar.Tests.Unit.Services.Fixtures;
using Xunit;

namespace Calendar.Tests.Unit.CompactModels.ActivityAllocationSpec
{
    [XUnitCases]
    public class when_contains_no_activity : DbSpec
    {
        private static DayActivity day;

        [Fact]
        public void test_when_contains_no_activity()
        {
            Given("context", () => day = new DayActivity(Activity.Empty(), Activity.Empty(), new LocationPattern(LocationValues.China), 1, new List<string>()));
            Then("should_use_default_allocation_rule", () =>
            {
                var allocationRule = new ActivityAllocationRule(day);
                Assert.True(allocationRule.IsFirstActivityFirstLocationAllocationEmpty());
                Assert.True(allocationRule.IsSecondActivityFirstLocationAllocationEmpty());
            });
        }
    }
}