using System;
using Calendar.Model.Compact;
using Xunit;

namespace Calendar.Tests.Unit.CompactModels
{
    [XUnitCases]
    public class user_feed : DbSpec
    {
        [Fact]
        public void test_user_feed()
        {
            Then("should", () =>
            {
                var feedEntryId = Guid.NewGuid();
                var userId = Guid.NewGuid();
                var userFeed = new UserFeed(feedEntryId, userId, new DateTime(2012, 1, 1, 0, 0, 0), new DateTime(2012, 1, 1, 1, 0, 0));
                var ticks = DateTime.Now.Ticks;
                userFeed.AddDay(new DateTime(2012, 1, 1), ticks);
                userFeed.AddDay(new DateTime(2012, 1, 1), ticks + 1);
                var syndicationItem = userFeed.ToSyndicationItem("http://localhost");
                Assert.StartsWith("Calendar Changed of User", syndicationItem.Title.Text);
                Assert.Equal(feedEntryId.ToString(), syndicationItem.Id);
                Assert.Equal(new DateTime(ticks + 1, DateTimeKind.Utc), userFeed.LastUpdatedTime);
            });
        }
    }
}