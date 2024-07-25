using System;
using System.Collections.Generic;
using System.Linq;
using Calendar.Client.Schema;
using Calendar.General.Persistence;
using Calendar.Model.Compact;
using Calendar.Persistence;
using Calendar.Service.Controller;
using Calendar.Tests.Unit.CompactModels;
using Calendar.Tests.Unit.Services.Fixtures;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Note = Calendar.Model.Compact.Note;

namespace Calendar.Tests.Unit.ArchiveAssignee
{
    [XUnitCases]
    public class get_assignee_calendar_archived_data : DbSpec
    {
        private readonly ISessionWrapper session;
        private static readonly Guid UserId = Guid.NewGuid();
        private static readonly DateTime Date = new DateTime(2011, 5, 5);
        private static readonly DateTime FirstTravelDepartureDateTime = new DateTime(2011, 5, 5, 12, 10, 0);
        private static readonly DateTime FirstTravelArrivalDateTime = new DateTime(2011, 5, 5, 14, 5, 0);
        private static ArchiveAssigneeController controller;

        public get_assignee_calendar_archived_data()
        {
            session = DbHelper.GetSession();
            cleanup_db();
        }

        private void VerifyCalendarDaysInfo(HashSet<Day> days)
        {
            Assert.Single(days);
            foreach (Day day in days)
            {
                Assert.Equal(LocationValues.California, day.Location);
                Assert.Equal("2011-05-05", day.Date);
                Assert.Equal("content", day.Note.Content);

                Assert.Equal(2, day.Activities.Count);
                Assert.Equal(Calendar.Models.ActivityType.Work, day.Activities.First().Type);
                Assert.Equal(1, day.Activities.First().FirstAllocation);
                Assert.Equal(0, day.Activities.First().SecondAllocation);

                Assert.Equal(Calendar.Models.ActivityType.Sick, day.Activities.Last().Type);
                Assert.Equal(0, day.Activities.Last().FirstAllocation);
                Assert.Equal(1, day.Activities.Last().SecondAllocation);

                Assert.Single(day.Travels);
                Assert.Equal("2011-05-05", day.Travels.First().Departure.Date);
                Assert.Equal(LocationValues.California, day.Travels.First().Departure.Location);
                Assert.Equal("1200", day.Travels.First().Departure.Time);
                Assert.Equal("2011-05-05", day.Travels.First().Arrival.Date);
                Assert.Equal(LocationValues.China, day.Travels.First().Arrival.Location);
                Assert.Equal("1400", day.Travels.First().Arrival.Time);
            }
        }

        private void cleanup_db()
        {
            Cleaner.CleanTable(session, typeof(Note), typeof(MonthActivity));
        }

        private void PrepareAssigneeCalendarData()
        {
            var monthActivity = new MonthActivity(UserId, Date.Year, Date.Month);
            var dayActivity = new DayActivity(new Model.Compact.Activity(ActivityType.Work, 100),
                new Model.Compact.Activity(ActivityType.Sick, 0),
                new LocationPattern(LocationValues.California, null, FirstTravelDepartureDateTime.Hour,
                    LocationValues.China, FirstTravelArrivalDateTime.Hour), Date.Day, new List<string>());
            monthActivity.Update(dayActivity);
            session.SaveOrUpdateAndFlush(monthActivity);

            session.SaveOrUpdateAndFlush(new Note(UserId, Date, "content"));
        }


        [Fact]
        public void test_get_assignee_calendar_archived_data()
        {
            Given("given_assignee_has_calendar_data", () =>
            {
                PrepareAssigneeCalendarData();

                controller = new ArchiveAssigneeController();
            });
            When("get_assignee_archived_data", () => { });
            var result = controller.Get(UserId.ToString());
            var value = ((OkObjectResult)result.Result)?.Value as TableInfoDto;
            Assert.NotNull(result);
            Assert.NotNull(value);
            VerifyCalendarDaysInfo(value.All.CalendarInfo.Days);
            VerifyCalendarDaysInfo(value.NonPii.CalendarInfo.Days);
            cleanup_db();
        }
    }
}