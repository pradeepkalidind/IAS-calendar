using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;

namespace Calendar.API.Test.Settings
{
    public class SettingsFacts : ApiFactBase
    {
        private readonly Guid userId = new Guid("c1efe2b1-3477-42ef-bbfb-b673ffdb0478");

        [Fact]
        public async Task should_save_workWeekDefaults()
        {
            var response = await GetSettingsClient().SaveWorkWeekDefaults(userId, string.Empty);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("{\"wwd\":[]}", await GetSettingsClient().GetCalendarSettings(userId));
        }

        [Fact]
        public async Task should_save_defaultWorkingDays()
        {
            var @params = JsonConvert.SerializeObject(new
            {
                days = 1, userId,
            });
            var response = await GetSettingsClient().SaveDefaultWorkingDays(userId, @params);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}