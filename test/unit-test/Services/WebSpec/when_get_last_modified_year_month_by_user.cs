using System;
using Calendar.Model.Compact;
using Calendar.Service.Services;
using Calendar.Tests.Unit.CompactModels.LocationRulesSpec;
using Xunit;

namespace Calendar.Tests.Unit.Services.WebSpec
{
    [Collection("NotSharedDatabaseCollection")]
    public class when_get_last_modified_year_month_by_user : CompactModelSpec
    {
        private static Guid kaylaId;

        public when_get_last_modified_year_month_by_user()
        {
            kaylaId = Guid.NewGuid();
            var lastModifiedYearMonth = new LastModifiedYearMonth(kaylaId);
            lastModifiedYearMonth.LastModifiedMonth = 5;
            lastModifiedYearMonth.LastModifiedYear = 2018;
            Session.Save(lastModifiedYearMonth);
        }

        [Fact]
        public void should_get_visit_status_for_kayla()
        {
            var service = new LastModifiedYearMonthService(Session);
            var lastModifiedYearMonth = service.GetByUser(kaylaId);
            Assert.Equal(kaylaId, lastModifiedYearMonth.UserId);
            Assert.Equal(5, lastModifiedYearMonth.LastModifiedMonth);
            Assert.Equal(2018, lastModifiedYearMonth.LastModifiedYear);
        }

        [Fact]
        public void should_return_null_for_user_has_not_visited()
        {
            Assert.Null(new LastModifiedYearMonthService(Session).GetByUser(Guid.NewGuid()));
        }
    }
}
