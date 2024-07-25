using Calendar.Model.Compact;
using Xunit;

namespace Calendar.Tests.Unit.CompactModels
{
    [XUnitCases]
    public class activity_type_convertor : DbSpec
    {
        [Fact]
        public void test_activity_type_convertor()
        {
            Then("should_", () =>
            {
                Assert.Equal(ActivityType.Work, ActivityTypeConvertor.GetActivityType("W"));
                Assert.Equal(ActivityType.COVID19Work, ActivityTypeConvertor.GetActivityType("CW"));
                Assert.Equal(ActivityType.NonWork, ActivityTypeConvertor.GetActivityType("NW"));
                Assert.Equal(ActivityType.COVID19NonWork, ActivityTypeConvertor.GetActivityType("CNW"));
                Assert.Equal(ActivityType.National, ActivityTypeConvertor.GetActivityType("N"));
                Assert.Equal(ActivityType.Sick, ActivityTypeConvertor.GetActivityType("S"));
                Assert.Equal(ActivityType.Vacation, ActivityTypeConvertor.GetActivityType("V"));
                Assert.Equal(ActivityType.Empty, ActivityTypeConvertor.GetActivityType("E"));
                Assert.Equal(ActivityType.Empty, ActivityTypeConvertor.GetActivityType(""));
            });
            Then("should_get_activity_long_name", () =>
            {
                Assert.Equal("/ActivityType/Work", ActivityTypeConvertor.GetActivityLongName(ActivityType.COVID19Work));
                Assert.Equal("/ActivityType/NonWork", ActivityTypeConvertor.GetActivityLongName(ActivityType.COVID19NonWork));
                Assert.Equal("/ActivityType/Work", ActivityTypeConvertor.GetActivityLongName(ActivityType.Work));
                Assert.Equal("/ActivityType/NonWork", ActivityTypeConvertor.GetActivityLongName(ActivityType.NonWork));
                Assert.Equal("/ActivityType/National", ActivityTypeConvertor.GetActivityLongName(ActivityType.National));
                Assert.Equal("/ActivityType/Sick", ActivityTypeConvertor.GetActivityLongName(ActivityType.Sick));
                Assert.Equal("/ActivityType/Vacation", ActivityTypeConvertor.GetActivityLongName(ActivityType.Vacation));
                Assert.Equal("/ActivityType/Empty", ActivityTypeConvertor.GetActivityLongName(ActivityType.Empty));
            });
            Then("should_get_activity_by_long_name", () =>
            {
                Assert.Equal(ActivityType.COVID19Work, ActivityTypeConvertor.GetActivityTypeByLongName("/ActivityType/Work", true));
                Assert.Equal(ActivityType.COVID19NonWork, ActivityTypeConvertor.GetActivityTypeByLongName("/ActivityType/NonWork", true));
                Assert.Equal(ActivityType.Work, ActivityTypeConvertor.GetActivityTypeByLongName("/ActivityType/Work", false));
                Assert.Equal(ActivityType.NonWork, ActivityTypeConvertor.GetActivityTypeByLongName("/ActivityType/NonWork", false));
                Assert.Equal(ActivityType.National, ActivityTypeConvertor.GetActivityTypeByLongName("/ActivityType/National", false));
                Assert.Equal(ActivityType.Sick, ActivityTypeConvertor.GetActivityTypeByLongName("/ActivityType/Sick", false));
                Assert.Equal(ActivityType.Vacation, ActivityTypeConvertor.GetActivityTypeByLongName("/ActivityType/Vacation", false));
                Assert.Equal(ActivityType.Empty, ActivityTypeConvertor.GetActivityTypeByLongName("/ActivityType/Empty", false));
            });
            Then("should_get_is_covid_by_short_name", () =>
            {
                Assert.True(ActivityTypeConvertor.GetIsCovidByActivityShortName("CW"));
                Assert.True(ActivityTypeConvertor.GetIsCovidByActivityShortName("CNW"));
                Assert.False(ActivityTypeConvertor.GetIsCovidByActivityShortName("W"));
                Assert.False(ActivityTypeConvertor.GetIsCovidByActivityShortName("NW"));
                Assert.False(ActivityTypeConvertor.GetIsCovidByActivityShortName("N"));
                Assert.False(ActivityTypeConvertor.GetIsCovidByActivityShortName("S"));
                Assert.False(ActivityTypeConvertor.GetIsCovidByActivityShortName("V"));
                Assert.False(ActivityTypeConvertor.GetIsCovidByActivityShortName("E"));
            });
        }
    }
}