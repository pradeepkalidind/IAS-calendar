using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;

namespace Calendar.API.Test.UserCalendar
{
    public class UserCalendarFacts : ApiFactBase
    {
        private readonly Guid userId = new Guid("32e11ce2-eb60-4eb8-a24b-d8f76352f71f");

        [Fact]
        public async Task should_get_user_calendar()
        {
            const string daysBody =
                "[{\"D\":\"2012/01/01\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"US\",\"N\":\"note\"},{\"D\":\"2012/01/02\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"US\",\"N\":\"note\"}]";
            const string daysBodyDelete = "[{\"D\":\"2012/01/02\"}]";
            var result = await Do(daysBody, daysBodyDelete);
            Assert.Equal(
                "{\"Days\":[{\"Date\":\"2012-01-01\",\"Location\":\"/Country/US\",\"ArrivalTime\":null,\"DepartureTime\":null,\"IsCovid\":\"FALSE\",\"Activities\":[{\"Type\":\"/ActivityType/NonWork\",\"FirstAllocation\":1.0,\"SecondAllocation\":0.0,\"FirstAllocationAttr\":\"1\",\"SecondAllocationAttr\":\"0\"},{\"Type\":\"/ActivityType/NonWork\",\"FirstAllocation\":1.0,\"SecondAllocation\":0.0,\"FirstAllocationAttr\":\"1\",\"SecondAllocationAttr\":\"0\"}],\"Note\":{\"Content\":\"note\"},\"Travels\":[]}],\"IasPlatformUser\":\"32e11ce2-eb60-4eb8-a24b-d8f76352f71f\"}",
                result);
            Assert.Equal(
                "{\"Days\":[{\"Date\":\"2012-01-01\",\"Location\":\"/Country/US\",\"ArrivalTime\":null,\"DepartureTime\":null,\"IsCovid\":\"FALSE\",\"Activities\":[{\"Type\":\"/ActivityType/NonWork\",\"FirstAllocation\":1.0,\"SecondAllocation\":0.0,\"FirstAllocationAttr\":\"1\",\"SecondAllocationAttr\":\"0\"},{\"Type\":\"/ActivityType/NonWork\",\"FirstAllocation\":1.0,\"SecondAllocation\":0.0,\"FirstAllocationAttr\":\"1\",\"SecondAllocationAttr\":\"0\"}],\"Note\":{\"Content\":\"note\"},\"Travels\":[]}],\"IasPlatformUser\":\"32e11ce2-eb60-4eb8-a24b-d8f76352f71f\"}",
                await DoByDate("new"));
            Assert.Equal(
                "{\"Days\":[{\"Date\":\"2012-01-01\",\"Location\":\"/Country/US\",\"ArrivalTime\":null,\"DepartureTime\":null,\"IsCovid\":\"FALSE\",\"Activities\":[{\"Type\":\"/ActivityType/NonWork\",\"FirstAllocation\":1.0,\"SecondAllocation\":0.0,\"FirstAllocationAttr\":\"1\",\"SecondAllocationAttr\":\"0\"},{\"Type\":\"/ActivityType/NonWork\",\"FirstAllocation\":1.0,\"SecondAllocation\":0.0,\"FirstAllocationAttr\":\"1\",\"SecondAllocationAttr\":\"0\"}],\"Note\":{\"Content\":\"note\"},\"Travels\":[]}],\"DeletedDays\":[{\"Date\":\"2012-01-02\"}],\"IasPlatformUser\":\"32e11ce2-eb60-4eb8-a24b-d8f76352f71f\"}",
                await DoByDates("new"));
        }

        private async Task SaveDays(string daysBody, string daysBodyDelete)
        {
            var daysBodyParam = "daysBody=" + daysBody;
            await GetClient("").RetrieveData("Calendar/SaveDays", userId, daysBodyParam, false);
            daysBodyParam = "daysBody=" + daysBodyDelete;
            await GetClient("").RetrieveData("Calendar/SaveDays", userId, daysBodyParam, false);
        }

        private async Task<string> Do(string daysBody, string daysBodyDelete)
        {
            await SaveDays(daysBody, daysBodyDelete);
            var response = await GetClient("").UserCalendarAll(userId);
            return JsonConvert.SerializeObject(response);
        }

        private async Task<string> DoByDate(string area)
        {
            var response = await GetClient("").UserCalendar(userId, "2012-01-01", "2012-01-01");
            return JsonConvert.SerializeObject(response);
        }

        private async Task<string> DoByDates(string area)
        {
            var response = await GetClient("").UserCalendar(userId, DateTime.UtcNow.AddHours(-1).Ticks, DateTime.UtcNow.Ticks);
            return JsonConvert.SerializeObject(response);
        }
    }
}