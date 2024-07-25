using System;
using System.Linq;
using Calendar.Model.Compact;
using Calendar.Service.Services;
using Calendar.Tests.Unit.CompactModels.LocationRulesSpec;
using Xunit;

namespace Calendar.Tests.Unit.Services.WebSpec
{
    [Collection("NotSharedDatabaseCollection")]
    public class when_mark_user_as_visited : CompactModelSpec
    {
        private static Guid notVisitedUserId;
        private static Guid visitedUserId;
        private static VisitedUserService service;

        public when_mark_user_as_visited()
        {
            visitedUserId = Guid.NewGuid();
            notVisitedUserId = Guid.NewGuid();
            Session.Save(new VisitedUser(visitedUserId));

            service = new VisitedUserService(Session);
        }

        [Fact]
        public void should_mark_new_user_as_visited()
        {
            service.MarkAsVisited(notVisitedUserId);

            var visitStatus = Session.Query<VisitedUser>().Where(s => s.UserId == notVisitedUserId).ToList();

            Assert.Single(visitStatus);
        }

        [Fact]
        public void should_do_nothing_when_mark_visited_user_visit()
        {
            service.MarkAsVisited(visitedUserId);

            var visitStatus = Session.Query<VisitedUser>().Where(s => s.UserId == visitedUserId).ToList();
            Assert.Single(visitStatus);
        }
    }
}