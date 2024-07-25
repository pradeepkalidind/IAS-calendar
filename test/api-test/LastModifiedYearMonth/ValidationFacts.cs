using System;
using System.Threading.Tasks;
using Xunit;

namespace Calendar.API.Test.LastModifiedYearMonth
{
    public class ValidationFacts : ApiFactBase
    {
        private readonly Guid userId = Guid.NewGuid();

        [Fact]
        public async Task should_be_valid_month()
        {
            Assert.Equal("BadRequest", await SetLastModifiedYearMonth("2019", "13"));
        }

        [Fact]
        public async Task should_be_positive_year()
        {
            Assert.Equal("BadRequest", await SetLastModifiedYearMonth("0", "5"));
        }

        [Fact]
        public async Task should_not_be_empty_month()
        {
            Assert.Equal("BadRequest", await SetLastModifiedYearMonth("2011", ""));
        }

        [Fact]
        public async Task should_not_be_empty_year()
        {
            Assert.Equal("BadRequest", await SetLastModifiedYearMonth("", "5"));
        }

        private async Task<string> SetLastModifiedYearMonth(string year, string month)
        {
            var response = await GetClient("").SetLastModifiedYearMonth(userId, year, month);
            return response.StatusCode.ToString();
        }
    }
}