using System;
using System.Linq;
using Calendar.Model.Compact;
using Calendar.Persistence;
using Xunit;

namespace Calendar.Tests.Unit.CompactModels
{
    [XUnitCases]
    public class save_and_retrive_month : DbSpec
    {
        private static ISessionWrapper session;

        private static readonly Guid UserId = new Guid("C42D18E1-8B1F-43BD-8DCC-B75055F49AF1");

        private static void cleanup_db()
        {
            Cleaner.CleanTable(DbHelper.GetSession(), typeof(MonthActivity));
        }

        [Fact]
        public void test_save_and_retrive_month()
        {
            cleanup_db();
            Given("context", () =>
            {
                session = DbHelper.GetSession();
                var monthActivity = new MonthActivity(UserId, 2012, 1);
                // monthActivity.WithDayActivitiy(1, ActivityType.Work, AllocationType.T0);
                session.Save(monthActivity);
            });
            Then("should_retrive", () =>
            {
                var monthActivities = session.Query<MonthActivity>();
                Assert.True(monthActivities.Any());
                var monthActivity = monthActivities.First();

                Assert.Equal(31, monthActivity.GetDays().Length);
            });
        }
    }
}