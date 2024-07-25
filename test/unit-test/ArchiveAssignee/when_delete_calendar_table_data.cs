using System;
using System.Linq;
using Calendar.Client.Schema;
using Calendar.General.Persistence;
using Calendar.Model.Compact;
using Calendar.Persistence;
using Calendar.Service.Controller;
using Calendar.Tests.Unit.CompactModels;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Note = Calendar.Model.Compact.Note;

namespace Calendar.Tests.Unit.ArchiveAssignee
{
    [XUnitCases]
    public class when_delete_calendar_table_data : DbSpec
    {
        static ArchiveAssigneeController controller;
        protected static Guid UserId = Guid.NewGuid();
        protected static DateTime Date = new DateTime(2011, 5, 5);
        protected static DateTime FirstTravelDepartureDateTime = new DateTime(2011, 5, 5, 12, 10, 0);
        protected static DateTime FirstTravelArrivalDateTime = new DateTime(2011, 5, 5, 14, 5, 0);
        protected static DateTime SecondTravelDepartureDateTime = new DateTime(2011, 5, 5, 14, 15, 0);
        protected static DateTime SecondTravelArrivalDateTime = new DateTime(2011, 5, 5, 16, 5, 0);

        protected static UserCalendar UserCalendar;

        static ISessionWrapper session;

        public when_delete_calendar_table_data()
        {
            controller = new ArchiveAssigneeController();
            session = DbHelper.GetSession();
            cleanup_db();
        }

        [Fact]
        public void test_when_delete_calendar_table_data()
        {
            var monthActivity = new MonthActivity(UserId, Date.Year, Date.Month);
            var defaultWorkDays = new DefaultWorkDays { UserId = UserId };
            var nationalHolidaySetting = new NationalHolidaySetting(UserId, "CN");
            var note = new Note(UserId, DateTime.UtcNow, "");
            var userActivity = new UserActivity(DateTime.UtcNow, UserId, 0L, "Tiger");
            session.SaveOrUpdateAndFlush(monthActivity);
            session.SaveOrUpdateAndFlush(defaultWorkDays);
            session.SaveOrUpdateAndFlush(nationalHolidaySetting);
            session.SaveOrUpdateAndFlush(note);
            session.SaveOrUpdateAndFlush(userActivity);

            var response = controller.Delete(UserId.ToString());

            var result = response as OkObjectResult;
            Assert.NotNull(response);
            Assert.NotNull(result.Value);

            Then("should_delete_table_data", () =>
            {
                Assert.Equal(0, session.Query<MonthActivity>().Count());
                Assert.Equal(0, session.Query<DefaultWorkDays>().Count());
                Assert.Equal(0, session.Query<NationalHolidaySetting>().Count());
                Assert.Equal(0, session.Query<Note>().Count());
                Assert.Equal(0, session.Query<UserActivity>().Count());
            });
            cleanup_db();
        }

        private void cleanup_db()
        {
            session.DeleteAll(typeof(MonthActivity));
            session.DeleteAll(typeof(DefaultWorkDays));
            session.DeleteAll(typeof(NationalHolidaySetting));
            session.DeleteAll(typeof(Note));
            session.DeleteAll(typeof(UserActivity));
        }
    }
}