using System;
using System.Threading.Tasks;
using Xunit;

namespace Calendar.API.Test;

public class RemoveDoubleSlashFacts: ApiFactBase
{
    private readonly Guid userId = Guid.NewGuid();
    
    [Fact]
    public async Task should_get_calendar_status_with_slashes()
    {
        const string daysBody =
            "[{\"D\":\"2012/01/01\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"beijing\"},{\"D\":\"2012/01/02\",\"A\":\"W\",\"SA\":\"W\",\"FL\":\"beijing\"}]";
        await SaveDays("new", daysBody);
        Assert.Equal(2, await GetCompleteDays("new", 2012));
    }

    private async Task SaveDays(string area, string daysBody)
    {
        var tokenParam = "token=" + userId;
        var daysBodyParam = "daysBody=" + daysBody;
        await GetClient(area).RetrieveData("Calendar///SaveDays", userId, daysBodyParam, false);
    }

    private async Task<int> GetCompleteDays(string area, int year)
    {
        return await GetClient(area).GetCompleteDays(userId, new DateTime(year, 1, 1), new DateTime(year, 12, 31));
    }
}