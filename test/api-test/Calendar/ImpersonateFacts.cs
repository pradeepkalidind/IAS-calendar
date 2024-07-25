using System;
using System.Threading.Tasks;
using PWC.IAS.Calendar.Client;
using Xunit;

namespace Calendar.API.Test.Calendar
{
    public class ImpersonateFacts : ApiFactBase
    {
        private readonly Guid userId = Guid.NewGuid();
        private readonly string testYear = "2012";

        [Theory]
        [InlineData("old",
            "{\"year\":[{\"y\":2012,\"m\":1,\"d\":[{\"D\":\"1\",\"A\":\"NW\",\"SA\":\"NW\",\"AA\":\"1\",\"SAA\":\"1\",\"FL\":\"US\",\"N\":\"note\"}],\"nt\":0},{\"y\":2012,\"m\":2,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":3,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":4,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":5,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":6,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":7,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":8,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":9,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":10,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":11,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":12,\"d\":[],\"nt\":0}]}")]
        [InlineData("new",
            "{\"year\":[{\"y\":2012,\"m\":1,\"d\":[{\"D\":\"1\",\"A\":\"NW\",\"SA\":\"NW\",\"AA\":\"1\",\"SAA\":\"1\",\"FL\":\"US\",\"N\":\"note\"}],\"nt\":0},{\"y\":2012,\"m\":2,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":3,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":4,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":5,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":6,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":7,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":8,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":9,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":10,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":11,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":12,\"d\":[],\"nt\":0}]}")]
        public async Task should_update_with_impersonate(string area, string expectedResult)
        {
            const string daysBody1 = "[{\"D\":\"2012/01/01\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"US\",\"N\":\"note\"}]";
            const string daysBody2 = "[{\"D\":\"2012/01/01\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"CN/BJ\",\"FLD\":\"10\",\"SL\":\"CN/GD\",\"SLA\":\"10\"}]";
            var result = await DoUpdateWithImpersonate(area, daysBody1, daysBody2, testYear);
            Assert.Equal(expectedResult, result);
        }

        private async Task<string> DoUpdateWithImpersonate(string area, string daysBody, string daysBody2, string year)
        {
            await SaveDays(daysBody, GetClient(area));
            await SaveDays(daysBody2, GetImpersonateClient(area));
            return await GetYear(year, area);
        }

        private async Task SaveDays(string daysBody, IasCalendarResource iasCalendarResource)
        {
            var daysBodyParam = "daysBody=" + daysBody;
            await iasCalendarResource.RetrieveData("Calendar/SaveDays", userId, daysBodyParam, false);
        }

        private async Task<string> GetYear(string year, string area)
        {
            var yearParam = "year=" + year;
            return await GetClient(area).RetrieveData("Calendar/GetYear", userId, yearParam, false);
        }
    }
}