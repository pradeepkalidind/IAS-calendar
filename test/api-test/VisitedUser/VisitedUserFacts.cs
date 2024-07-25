using System;
using System.Threading.Tasks;
using Xunit;

namespace Calendar.API.Test.VisitedUser
{
    public class VisitedUserFacts : ApiFactBase
    {
        private readonly Guid userId = Guid.NewGuid();

        [Fact]
        public async Task should_mark_as_visited()
        {
            Assert.Equal("false", await IsVisitedUser());
            await MarkAsVisited();
            Assert.Equal("true", await IsVisitedUser());
        }


        private async Task<string> IsVisitedUser()
        {
            return await GetClient("").GetIsVisitedUser(userId);
        }

        private async Task MarkAsVisited()
        {
            await GetClient("").MarkUserAsVisited(userId);
        }
    }
}