using System;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using Xunit;

namespace Calendar.API.Test.CalendarFeed
{
    public class GetFeedFacts : ApiFactBase
    {
        private readonly Guid userId = new Guid("a3450751-5736-4a8c-b5b3-9ca4762498b9");

        [Fact]
        public async Task should_get_feed()
        {
            const string daysBody = "[{\"D\":\"2012/01/01\",\"A\":\"NW\",\"SA\":\"NW\"}]";
            await SaveDays(daysBody);
            const string daysBody2 = "[{\"D\":\"2012/01/02\",\"A\":\"W\",\"SA\":\"W\"}]";
            await SaveDays(daysBody2);
            Assert.Equal(1, await GetFeedItemCount());
            Assert.Equal(
                "<IASPlatformUser Id=\"a3450751-5736-4a8c-b5b3-9ca4762498b9\"/><Days><Day>2012-01-02</Day><Day>2012-01-01</Day></Days>",
                await GetFeedFirstItemText());
            Assert.Equal(0, await GetArchivedFeedItemCount());
        }

        private async Task<int> GetArchivedFeedItemCount()
        {
            return (await GetClient("").CalendarChangedFeed(2011, 1, 1, 1, 1)).Items.Count();
        }

        private async Task SaveDays(string daysBody)
        {
            var daysBodyParam = "daysBody=" + daysBody;
            await GetClient("").RetrieveData("Calendar/SaveDays", userId, daysBodyParam, false);
        }

        private async Task<int> GetFeedItemCount()
        {
            return (await GetClient("").CalendarChangedFeed()).Items.Count();
        }

        private async Task<string> GetFeedFirstItemText()
        {
            var calendarChangedFeed = await GetClient("").CalendarChangedFeed();
            return ((TextSyndicationContent)calendarChangedFeed.Items.First().Content).Text;
        }
    }
}