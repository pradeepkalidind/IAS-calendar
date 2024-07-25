using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Calendar.API.Test.CalendarStatus
{
    public class GetCompleteDaysFacts : ApiFactBase
    {
        private readonly Guid userId = Guid.NewGuid();

        [Fact]
        public async Task should_get_calendar_status()
        {
            const string daysBody = "[{\"D\":\"2012/01/01\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"beijing\"},{\"D\":\"2012/01/02\",\"A\":\"W\",\"SA\":\"W\",\"FL\":\"beijing\"}]";
            await SaveDays("new", daysBody);
            Assert.Equal(2, await GetCompleteDays("new", 2012));
            Assert.Equal(2, await GetCompleteDaysByMonth("new", 2012, 1));
            Assert.False(await IsCalendarComplete("new", 2012, 1));
        }

        private async Task SaveDays(string area, string daysBody)
        {
            var tokenParam = "token=" + userId;
            var daysBodyParam = "daysBody=" + daysBody;
            await GetClient(area).RetrieveData("Calendar/SaveDays", userId, daysBodyParam, false);
        }

        private async Task<int> GetCompleteDays(string area, int year)
        {
            return await GetClient(area).GetCompleteDays(userId, new DateTime(year, 1, 1), new DateTime(year, 12, 31));
        }

        private async Task<int> GetCompleteDaysByMonth(string area, int year, int month)
        {
            var response = await GetClient(area)
                .GetCompleteDaysByMonth(
                    userId,
                    new DateTime(year, month, 1),
                    new DateTime(year, month, 31));
            return response.FirstOrDefault();
        }

        private async Task<bool> IsCalendarComplete(string area, int year, int month)
        {
            return await GetClient(area)
                .IsCalendarComplete(userId, new DateTime(year, month, 1), new DateTime(year, month, 1));
        }
    }
}