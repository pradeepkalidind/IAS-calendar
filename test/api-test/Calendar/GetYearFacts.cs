using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Calendar.API.Test.Calendar
{
    public class GetYearFacts : ApiFactBase
    {
        private readonly Guid userId = Guid.NewGuid();
        private const string testYear = "2012";

        private const string daysBody1 = "[{\"D\":\"2012/01/01\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"US\",\"N\":\"note\"}]";

        private const string daysBody2 = "[{\"D\":\"2012/01/01\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"CN/BJ\",\"FLD\":\"10\",\"SL\":\"CN/GD\",\"SLA\":\"10\"}]";


        [Theory]
        [InlineData("old",
            "{\"year\":[{\"y\":2012,\"m\":1,\"d\":[{\"D\":\"1\",\"A\":\"NW\",\"SA\":\"NW\",\"AA\":\"1\",\"SAA\":\"1\",\"FL\":\"US\",\"N\":\"note\"}],\"nt\":0},{\"y\":2012,\"m\":2,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":3,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":4,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":5,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":6,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":7,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":8,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":9,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":10,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":11,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":12,\"d\":[],\"nt\":0}]}")]
        [InlineData("new",
            "{\"year\":[{\"y\":2012,\"m\":1,\"d\":[{\"D\":\"1\",\"A\":\"NW\",\"SA\":\"NW\",\"AA\":\"1\",\"SAA\":\"1\",\"FL\":\"US\",\"N\":\"note\"}],\"nt\":0},{\"y\":2012,\"m\":2,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":3,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":4,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":5,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":6,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":7,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":8,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":9,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":10,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":11,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":12,\"d\":[],\"nt\":0}]}")]
        public async Task should_get_year(string area, string expectedResult)
        {
            const string daysBody = "[{\"D\":\"2012/01/01\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"US\",\"N\":\"note\"}]";
            await SaveDays(daysBody, area);
            var result =await GetYear(testYear, area);
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("old",
            "{\"year\":[{\"y\":2012,\"m\":1,\"d\":[{\"D\":\"1\",\"A\":\"NW\",\"SA\":\"NW\",\"AA\":\"1\",\"SAA\":\"0\",\"FL\":\"CN/BJ\",\"FLD\":\"10\",\"SL\":\"CN/GD\",\"SLA\":\"10\"}],\"nt\":0},{\"y\":2012,\"m\":2,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":3,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":4,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":5,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":6,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":7,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":8,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":9,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":10,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":11,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":12,\"d\":[],\"nt\":0}]}")]
        [InlineData("new",
            "{\"year\":[{\"y\":2012,\"m\":1,\"d\":[{\"D\":\"1\",\"A\":\"NW\",\"SA\":\"NW\",\"AA\":\"1\",\"SAA\":\"0\",\"FL\":\"CN/BJ\",\"FLD\":\"10\",\"SL\":\"CN/GD\",\"SLA\":\"10\"}],\"nt\":0},{\"y\":2012,\"m\":2,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":3,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":4,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":5,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":6,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":7,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":8,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":9,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":10,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":11,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":12,\"d\":[],\"nt\":0}]}")]
        public async Task should_update(string area, string expectedResult)
        {
            await SaveDays(daysBody1, area);
            await SaveDays(daysBody2, area);
            var result = await GetYear(testYear, area);
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task should_get_travel_data_for_latest_2_years()
        {
            const string daysBody = "[{\"D\":\"2012/01/01\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"CN/BJ\",\"FLD\":\"10\",\"SL\":\"ZA\",\"SLA\":\"10\"},{\"D\":\"2012/02/25\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"ZA\",\"FLD\":\"10\",\"SL\":\"CN/BJ\",\"SLA\":\"10\"},{\"D\":\"2013/02/01\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"CN/BJ\",\"FLD\":\"10\",\"SL\":\"CN/GD\",\"SLA\":\"10\"}]";
            const string expected = "[{\"year\":2013,\"countries\":{\"CN\":1.0}},{\"year\":2012,\"countries\":{\"ZA\":1.0,\"CN\":1.0}}]";
            
            Assert.Equal(expected,await GetKaylasLatestTravelYears("new", daysBody));
        }

        [Fact]
        public async Task should_get_travel_data_for_the_latest_2_years_without_inactive_year()
        {
            const string daysBody = "[{\"D\":\"2015/01/01\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"\",\"FLD\":\"0\",\"SL\":\"\",\"SLA\":\"\"},{\"D\":\"2014/02/01\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"CN/BJ\",\"FLD\":\"10\",\"SL\":\"CN/GD\",\"SLA\":\"10\"},{\"D\":\"2013/01/01\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"CN/BJ\",\"FLD\":\"10\",\"SL\":\"ZA\",\"SLA\":\"10\"},{\"D\":\"2013/02/25\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"ZA\",\"FLD\":\"10\",\"SL\":\"CN/BJ\",\"SLA\":\"10\"}]";
            const string expected = "[{\"year\":2014,\"countries\":{\"CN\":1.0}},{\"year\":2013,\"countries\":{\"ZA\":1.0,\"CN\":1.0}}]";
            Assert.Equal(expected,await GetKaylasLatestTravelYears("new", daysBody));
        }

        [Fact]
        public async Task should_get_travel_data_for_the_fisrt_actice_year_without_previous_inactive_year_or_next_incoming_inactive_year()
        {
            const string daysBody =
                "[{\"D\":\"2015/01/01\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"\",\"FLD\":\"0\",\"SL\":\"\",\"SLA\":\"\"},{\"D\":\"2014/02/01\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"CN/BJ\",\"FLD\":\"10\",\"SL\":\"CN/GD\",\"SLA\":\"10\"},{\"D\":\"2012/01/01\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"CN/BJ\",\"FLD\":\"10\",\"SL\":\"ZA\",\"SLA\":\"10\"},{\"D\":\"2012/02/25\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"ZA\",\"FLD\":\"10\",\"SL\":\"CN/BJ\",\"SLA\":\"10\"}]";
            Assert.Equal("[{\"year\":2014,\"countries\":{\"CN\":1.0}}]",await GetKaylasLatestTravelYears("new", daysBody));
        }

        [Fact]
        public async Task should_get_travel_data_for_two_sequential_active_years_without_previous_inactive_years()
        {
            const string daysBody = "[{\"D\":\"2015/01/01\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"\",\"FLD\":\"0\",\"SL\":\"\",\"SLA\":\"\"},{\"D\":\"2014/01/01\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"\",\"FLD\":\"0\",\"SL\":\"\",\"SLA\":\"\"},{\"D\":\"2012/02/01\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"CN/BJ\",\"FLD\":\"10\",\"SL\":\"CN/GD\",\"SLA\":\"10\"},{\"D\":\"2011/01/01\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"CN/BJ\",\"FLD\":\"10\",\"SL\":\"ZA\",\"SLA\":\"10\"},{\"D\":\"2011/02/25\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"ZA\",\"FLD\":\"10\",\"SL\":\"CN/BJ\",\"SLA\":\"10\"}]";
            const string expected = "[{\"year\":2012,\"countries\":{\"CN\":1.0}},{\"year\":2011,\"countries\":{\"ZA\":1.0,\"CN\":1.0}}]";
            Assert.Equal(expected,await GetKaylasLatestTravelYears("new", daysBody));
        }
        
        [Fact]
        public async Task should_get_travel_data_for_first_active_year_without_previous()
        {
            const string daysBody = "[{\"D\":\"2015/01/01\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"\",\"FLD\":\"0\",\"SL\":\"\",\"SLA\":\"\"},{\"D\":\"2014/01/01\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"\",\"FLD\":\"0\",\"SL\":\"\",\"SLA\":\"\"},{\"D\":\"2012/02/01\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"CN/BJ\",\"FLD\":\"10\",\"SL\":\"CN/GD\",\"SLA\":\"10\"},{\"D\":\"2011/01/01\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"CN/BJ\",\"FLD\":\"10\",\"SL\":\"ZA\",\"SLA\":\"10\"},{\"D\":\"2011/02/25\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"ZA\",\"FLD\":\"10\",\"SL\":\"CN/BJ\",\"SLA\":\"10\"}]";
            const string expected = "[{\"year\":2012,\"countries\":{\"CN\":1.0}},{\"year\":2011,\"countries\":{\"ZA\":1.0,\"CN\":1.0}}]";
            Assert.Equal(expected,await GetKaylasLatestTravelYears("new", daysBody));
        }

        [Theory]
        [InlineData(
            "[{\"D\":\"2015/02/01\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"CN/BJ\",\"FLD\":\"10\",\"SL\":\"CN/GD\",\"SLA\":\"10\"},{\"D\":\"2014/01/01\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"\",\"FLD\":\"0\",\"SL\":\"\",\"SLA\":\"\"},{\"D\":\"2013/01/01\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"CN/BJ\",\"FLD\":\"10\",\"SL\":\"ZA\",\"SLA\":\"10\"},{\"D\":\"2013/02/25\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"ZA\",\"FLD\":\"10\",\"SL\":\"CN/BJ\",\"SLA\":\"10\"},{\"D\":\"2012/01/01\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"\",\"FLD\":\"0\",\"SL\":\"\",\"SLA\":\"\"}]")]
        [InlineData(
            "[{\"D\":\"2015/02/01\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"CN/BJ\",\"FLD\":\"10\",\"SL\":\"CN/GD\",\"SLA\":\"10\"},{\"D\":\"2013/01/01\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"\",\"FLD\":\"0\",\"SL\":\"\",\"SLA\":\"\"},{\"D\":\"2012/01/01\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"\",\"FLD\":\"0\",\"SL\":\"\",\"SLA\":\"\"}]")]
        public async Task should_get_travel_data_for_first_active_year_without_previous_inactive_years(string daysBody)
        {
            const string expected = "[{\"year\":2015,\"countries\":{\"CN\":1.0}}]";
            Assert.Equal(expected, await GetKaylasLatestTravelYears("new", daysBody));
        }

        [Fact]
        public async Task should_get_travel_data_for_null_when_user_is_not_exist()
        {
            var response = await GetClient("").RetrieveTravelYearsResponse("Calendar/GetTravelData", Guid.Empty);
            Assert.Null(Record.Exception(() => response.EnsureSuccessStatusCode()));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Null(await response.Content.ReadFromJsonAsync<object>());
        }

        private async Task<string> GetKaylasLatestTravelYears(string area, string days)
        {
            await SaveDays(days, area);
            var result =await GetClient("").RetrieveTravelYears("Calendar/GetTravelData", userId);
            var travelInfos = result.Substring(result.IndexOf('['), result.LastIndexOf(']') - result.IndexOf('[') + 1);
            return travelInfos;
        }

        private async Task SaveDays(string daysBody, string area)
        {
            var daysBodyParam = "daysBody=" + daysBody;
            await GetClient(area).RetrieveData("Calendar/SaveDays", userId, daysBodyParam, false);
        }

        private async Task<string> GetYear(string year, string area)
        {
            var yearParam = "year=" + year;
            return await GetClient(area).RetrieveData("Calendar/GetYear", userId, yearParam, false);
        }
    }
}