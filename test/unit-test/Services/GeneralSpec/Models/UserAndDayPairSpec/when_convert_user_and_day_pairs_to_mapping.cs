using System;
using System.Collections.Generic;
using Calendar.General.Models;
using Xunit;

namespace Calendar.Tests.Unit.Services.GeneralSpec.Models.UserAndDayPairSpec
{
    [XUnitCases]
    public class when_convert_user_and_day_pairs_to_mapping : DbSpec
    {
        private static readonly Guid UserId1 = Guid.NewGuid();
        private static readonly Guid UserId2 = Guid.NewGuid();
        private static readonly DateTime Day1 = new DateTime(2011, 6, 1);
        private static readonly DateTime Day2 = new DateTime(2011, 7, 1);
        private static List<UserAndDayPair> pairs;
        private static Dictionary<Guid, List<DateTime>> mapping;

        [Fact]
        public void test_when_convert_user_and_day_pairs_to_mapping()
        {
            Given("context", () => pairs = new List<UserAndDayPair>
            {
                new UserAndDayPair
                {
                    UserId = UserId1,
                    Day = Day1
                },
                new UserAndDayPair
                {
                    UserId = UserId1,
                    Day = Day2
                },
                new UserAndDayPair
                {
                    UserId = UserId2,
                    Day = Day1
                },
                new UserAndDayPair
                {
                    UserId = UserId2,
                    Day = Day2
                },
            });
            When("of", () => mapping = pairs.ToUserDaysMapping());
            Then("should_succeed", () =>
            {
                Assert.Equal(2, mapping.Keys.Count);
                Assert.Contains(UserId1, mapping.Keys);
                Assert.Contains(UserId2, mapping.Keys);
                Assert.Equal(2, mapping[UserId1].Count);
                Assert.Contains(Day1, mapping[UserId1]);
                Assert.Contains(Day2, mapping[UserId1]);
                Assert.Equal(2, mapping[UserId2].Count);
                Assert.Contains(Day1, mapping[UserId2]);
                Assert.Contains(Day2, mapping[UserId2]);
            });
        }
    }
}