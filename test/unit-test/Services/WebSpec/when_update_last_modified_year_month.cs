using System;
using System.Linq;
using Calendar.Model.Compact;
using Calendar.Service.Services;
using Calendar.Tests.Unit.CompactModels.LocationRulesSpec;
using Xunit;

namespace Calendar.Tests.Unit.Services.WebSpec
{
    [Collection("NotSharedDatabaseCollection")]
    public class when_update_last_modified_year_month : CompactModelSpec
    {
        private static Guid notVisitedUserId;
        private static Guid visitedUserId;
        private static LastModifiedYearMonthService service;

        public when_update_last_modified_year_month()
        {
            visitedUserId = Guid.NewGuid();
            notVisitedUserId = Guid.NewGuid();

            Session.Save(new LastModifiedYearMonth(visitedUserId)
            {
                LastModifiedMonth = 5,
                LastModifiedYear = 2018
            });

            service = new LastModifiedYearMonthService(Session);
        }

        [Fact]
        public void should_update_last_modified_month_for_existing_user()
        {
            service.SetLastModifiedMonth(visitedUserId, 2017, 3);

            var lastModifiedYearMonths = Session.Query<LastModifiedYearMonth>().Where(s => s.UserId == visitedUserId).ToList();
            
            Assert.Single(lastModifiedYearMonths);
            Assert.Equal(3, lastModifiedYearMonths[0].LastModifiedMonth);
            Assert.Equal(2017, lastModifiedYearMonths[0].LastModifiedYear);
        }

        [Fact]
        public void should_save_last_modified_month_for_new_user()
        {
            service.SetLastModifiedMonth(notVisitedUserId, 2017, 3);

            var lastModifiedYearMonths = Session.Query<LastModifiedYearMonth>().Where(s => s.UserId == notVisitedUserId).ToList();
            
            Assert.Single(lastModifiedYearMonths);
            Assert.Equal(3, lastModifiedYearMonths[0].LastModifiedMonth);
            Assert.Equal(2017, lastModifiedYearMonths[0].LastModifiedYear);
        }
    }
}