using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;

namespace Calendar.API.Test.LastModifiedYearMonth
{
    public class LastModifiedYearMonthFacts : ApiFactBase
    {
        private readonly Guid userId = Guid.NewGuid();

        [Fact]
        public async Task Should()
        {
            Assert.Equal(string.Empty, await GetLastModifiedYearMonth());
            await SetLastModifiedYearMonth("2019", "2");
            Assert.Equal("2019-2", await GetLastModifiedYearMonth());
        }


        private async Task<string> GetLastModifiedYearMonth()
        {
            var response =await GetClient("").GetLastModifiedYearMonth(userId);
            if (response == "")
            {
                return "";
            }

            var dto = JsonConvert.DeserializeObject<Model.Compact.LastModifiedYearMonth>(response);
            return $"{dto?.LastModifiedYear}-{dto?.LastModifiedMonth}";
        }

        private async Task SetLastModifiedYearMonth(string year, string month)
        {
           await GetClient("").SetLastModifiedYearMonth(userId, year, month);
        }
    }
}