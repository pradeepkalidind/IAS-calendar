using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;

namespace Calendar.API.Test.UserCalendar
{
    public class CrossUserCalendarFacts : ApiFactBase
    {
        private readonly Guid firstUserId = new Guid("12e11ce2-eb60-4eb8-a24b-d8f76352f71f");
        private readonly Guid secondUserId = new Guid("22e11ce2-eb60-4eb8-a24b-d8f76352f71f");
        
        [Fact]
        public async Task should_get_user_calendar()
        {
            const string firstUser = "[{\"D\":\"2012/01/01\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"NewYork\"}]";
            const string secondUser = "[{\"D\":\"2012/01/01\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"BeiJing\"}]";

            var result = await Do(firstUser, secondUser);
            Assert.Equal("{\"Users\":[{\"Days\":[{\"Date\":\"2012-01-01\",\"Location\":\"/Country/BeiJing\",\"ArrivalTime\":null,\"DepartureTime\":null,\"IsCovid\":\"FALSE\",\"Activities\":[{\"Type\":\"/ActivityType/NonWork\",\"FirstAllocation\":1.0,\"SecondAllocation\":0.0,\"FirstAllocationAttr\":\"1\",\"SecondAllocationAttr\":\"0\"},{\"Type\":\"/ActivityType/NonWork\",\"FirstAllocation\":1.0,\"SecondAllocation\":0.0,\"FirstAllocationAttr\":\"1\",\"SecondAllocationAttr\":\"0\"}],\"Note\":null,\"Travels\":[]}],\"IasPlatformUser\":\"12e11ce2-eb60-4eb8-a24b-d8f76352f71f\"}]}", result);
        }


        private async Task SaveDays(string daysBody, Guid userId)
        {
            var daysBodyParam = "daysBody=" + daysBody;
            await GetClient("").RetrieveData("Calendar/SaveDays", userId, daysBodyParam, false);
        }

        private async Task<string> Do(string daysBodyOfFirstUser, string daysBodyOfSecondUser)
        {
            await SaveDays(daysBodyOfFirstUser, secondUserId);
            await SaveDays(daysBodyOfSecondUser, firstUserId);
            var userDayPairs = new[]
            {
                new { UserId = firstUserId, Day = "2012-01-01" },
                new { UserId = secondUserId, Day = "2012-01-02" }
            };
            var response = await GetClient("").UserCalendar(JsonConvert.SerializeObject(userDayPairs));
            return JsonConvert.SerializeObject(response);
        }
    }
}