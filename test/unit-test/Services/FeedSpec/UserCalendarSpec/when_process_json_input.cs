using System;
using System.Collections.Generic;
using Calendar.General.Models;
using Xunit;

namespace Calendar.Tests.Unit.Services.FeedSpec.UserCalendarSpec
{
    [XUnitCases]
    public class when_process_json_input : DbSpec
    {
        private const string JSON = @"[{UserId: '9D325B17-BE7B-491D-B195-D0EF2EE1303B', Day: '2010-5-2'}, {UserId: '775FE479-7AA4-48FC-B94C-9B08242412EB', Day: '2010-5-2'}]";
        private static Dictionary<Guid, List<DateTime>> mapping;

        [Fact]
        public void test_when_process_json_input()
        {
            When("of", () => mapping = UserDaysMappingExtractor.Extract(JSON));
            Then("should_deserialize_to_mapping", () =>
            {
                Assert.Equal(2, mapping.Keys.Count);
                var firstUserId = Guid.Parse("9D325B17-BE7B-491D-B195-D0EF2EE1303B");
                var secondUserId = Guid.Parse("775FE479-7AA4-48FC-B94C-9B08242412EB");
                Assert.Contains(firstUserId, mapping.Keys);
                Assert.Contains(secondUserId, mapping.Keys);
                Assert.Contains(new DateTime(2010, 5, 2), mapping[firstUserId]);
                Assert.Contains(new DateTime(2010, 5, 2), mapping[secondUserId]);
            });
        }
    }
}