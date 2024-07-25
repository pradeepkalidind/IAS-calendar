using System;
using Calendar.Model.Compact;
using Calendar.Service.Services;
using Calendar.Tests.Unit.CompactModels.LocationRulesSpec;
using Xunit;

namespace Calendar.Tests.Unit.Services.WebSpec
{
    [Collection("NotSharedDatabaseCollection")]
    public class when_tell_if_user_is_visited_user : CompactModelSpec
    {
        private static Guid kaylaId;

        public when_tell_if_user_is_visited_user()
        {
            kaylaId = Guid.NewGuid();
            var visitStatus = new VisitedUser(kaylaId);
            Session.Save(visitStatus);
        }

        [Fact]
        public void should_return_true_if_user_has_visited()
        {
            var service = new VisitedUserService(Session);

            Assert.True(service.IsVisitedUser(kaylaId));
        }

        [Fact]
        public void should_return_null_for_user_has_not_visited()
        {
            Assert.False(new VisitedUserService(Session).IsVisitedUser(Guid.NewGuid()));
        }
    }
}
