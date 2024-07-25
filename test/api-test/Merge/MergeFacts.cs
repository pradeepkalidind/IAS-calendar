using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Calendar.API.Test.Merge
{
    public class MergeFacts : ApiFactBase
    {
        private readonly Guid sourceUserId = Guid.NewGuid();
        private readonly Guid destUserId = Guid.NewGuid();
        
        [Fact]
        public async Task should_merge()
        {
            const string daysBodyOfDestUser = "[{\"D\":\"2012/01/01\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"NewYork\"}]";
            const string daysBodyOfSourceUser = "[{\"D\":\"2012/01/01\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"BeiJing\"},{\"D\":\"2012/02/02\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"HeiBei\"}]";
            const string expected = "{\"year\":[{\"y\":2012,\"m\":1,\"d\":[{\"D\":\"1\",\"A\":\"NW\",\"SA\":\"NW\",\"AA\":\"1\",\"SAA\":\"1\",\"FL\":\"BeiJing\"}],\"nt\":0},{\"y\":2012,\"m\":2,\"d\":[{\"D\":\"2\",\"A\":\"NW\",\"SA\":\"NW\",\"AA\":\"1\",\"SAA\":\"1\",\"FL\":\"HeiBei\"}],\"nt\":0},{\"y\":2012,\"m\":3,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":4,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":5,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":6,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":7,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":8,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":9,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":10,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":11,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":12,\"d\":[],\"nt\":0}]}";
            Assert.Equal(expected,await Do("new", daysBodyOfDestUser, daysBodyOfSourceUser, "2012"));
        }

        private async Task<string> Do(string area, string daysBodyOfDestUser, string daysBodyOfSourceUser, string year)
        {
            await SaveDays(daysBodyOfDestUser, area, destUserId);
            Thread.Sleep(100);
            await SaveDays(daysBodyOfSourceUser, area, sourceUserId);
            await Merge(area);
            return await GetYear(year, area);
        }

        private async Task Merge(string area)
        {
            var param = "mytravelUserId=" + sourceUserId;
            param = param + "&" + "mytaxesUserId=" + destUserId;
            await GetClient(area).Merge("MergeCalendar/Merge", param, false);
        }

        private async Task<string> GetYear(string year, string area)
        {
            var yearParam = "year=" + year;
            return await GetClient(area).RetrieveData("Calendar/GetYear", destUserId, yearParam, false);
        }

        private async Task SaveDays(string daysBody, string area, Guid userId)
        {
            var daysBodyParam = "daysBody=" + daysBody;
            await GetClient(area).RetrieveData("Calendar/SaveDays", userId, daysBodyParam, false);
        }
    }
}