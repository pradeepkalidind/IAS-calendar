using System;
using System.Linq;
using System.Threading.Tasks;
using Calendar.General.Dto;
using Newtonsoft.Json;
using Xunit;

namespace Calendar.API.Test.Days
{
    public class DaysFacts : ApiFactBase
    {
        private readonly Guid userId = Guid.NewGuid();

        [Fact]
        public async Task should_get_days()
        {
            const string param1 =
                "{\"updatedDays\":[{\"firstActivity\":\"/ActivityType/Work\",\"date\":\"2012/01/01\"},{\"firstActivity\":\"/ActivityType/Work\",\"date\":\"2012/01/02\"}],\"deletedDays\":[]}";
            const string param2 =
                "{\"updatedDays\":[{\"firstActivity\":\"/ActivityType/Sick\",\"date\":\"2012/01/01\"}],\"deletedDays\":[{\"date\":\"2012/01/02\"}],\"enterFrom\":\"Mobile\"}";
            var result = await Do("new", param1, param2);
            Assert.Equal("/activitytype/sick incomplete mobile", result);
        }


        private async Task<string> Do(string area, string params1, string params2)
        {
            await SaveDays(params1);
            await SaveDays(params2);
            var daysRead = await GetClient("").DaysRead(userId.ToString(), "2012-01-01", "2012-01-02");
            var dto = JsonConvert.DeserializeObject<ChangedDaysDto>(daysRead);

            var dayDto = dto?.updatedDays.First();
            return $"{dayDto?.firstActivity} {dayDto?.type} {dayDto?.enterFrom}";
        }

        private async Task SaveDays(string @params)
        {
            await GetClient("").DaysSave(userId.ToString(), @params);
        }
    }
}