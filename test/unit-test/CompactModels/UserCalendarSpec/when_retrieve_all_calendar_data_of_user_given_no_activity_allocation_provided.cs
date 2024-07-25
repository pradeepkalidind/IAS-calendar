using System;
using System.Collections.Generic;
using System.Linq;
using Calendar.Client.Schema;
using Calendar.General.Persistence;
using Calendar.Model.Compact;
using Calendar.Model.Convertor;
using Calendar.Persistence;
using Calendar.Tests.Unit.Services.Fixtures;
using Xunit;
using Note = Calendar.Model.Compact.Note;

namespace Calendar.Tests.Unit.CompactModels.UserCalendarSpec
{
    [XUnitCases]
    public class when_retrieve_all_calendar_data_of_user_given_no_activity_allocation_provided: DbSpec
    {
        private static ISessionWrapper session;
        private static readonly Guid UserId = Guid.NewGuid();
        private static readonly DateTime Date = new DateTime(2011, 5, 5);
        private static readonly DateTime FirstTravelDepartureDateTime = new DateTime(2011, 5, 5, 12, 10, 0);
        private static readonly DateTime FirstTravelArrivalDateTime = new DateTime(2011, 5, 5, 14, 5, 0);
        private static readonly DateTime SecondTravelDepartureDateTime = new DateTime(2011, 5, 5, 14, 15, 0);
        private static readonly DateTime SecondTravelArrivalDateTime = new DateTime(2011, 5, 5, 16, 5, 0);
        private static UserCalendar userCalendar;

        private static void InitFixtures()
        {
            session = DbHelper.GetSession();
            var monthActivity = new MonthActivity(UserId, Date.Year, Date.Month);
            var dayActivity = new DayActivity(new Model.Compact.Activity(ActivityType.Work, 0),
                new Model.Compact.Activity(ActivityType.Sick, 0),
                new LocationPattern(LocationValues.California, null, FirstTravelDepartureDateTime.Hour,
                    LocationValues.China, FirstTravelArrivalDateTime.Hour, SecondTravelDepartureDateTime.Hour,
                    LocationValues.California, SecondTravelArrivalDateTime.Hour), Date.Day, new List<string>());
            monthActivity.Update(dayActivity);
            session.SaveOrUpdateAndFlush(monthActivity);

            session.SaveOrUpdateAndFlush(new Note(UserId, Date, "content"));
            
        }

        private void cleanup_db()
        {
            Cleaner.CleanTable(DbHelper.GetSession(), typeof(Note), typeof(MonthActivity));
        }

        [Fact]
        public void test_when_retrieve_all_calendar_data_of_user_given_no_activity_allocation_provided()
        {
            Given("context", InitFixtures);
            When("of", () => userCalendar = new CalendarDtoRetriever(session).Retrieve(UserId));
            Then("should_retrieve_all_calendar_data_of_user", () =>
            {
                Assert.Single(userCalendar.Days);
                Day day = userCalendar.Days.First();
                Assert.Equal(LocationValues.California, day.Location);
                Assert.Equal("2011-05-05", day.Date);
                Assert.Equal("content", day.Note.Content);

                Assert.Equal(2, day.Activities.Count);
                Assert.Equal(Models.ActivityType.Work, day.Activities.First().Type);
                Assert.Equal(0, day.Activities.First().FirstAllocation);
                Assert.Equal(1, day.Activities.First().SecondAllocation);
                Assert.Equal(Models.ActivityType.Sick, day.Activities.Last().Type);
                Assert.Equal(0, day.Activities.Last().FirstAllocation);
                Assert.Equal(1, day.Activities.Last().SecondAllocation);

                Assert.Equal(2, day.Travels.Count);
                Assert.Equal("2011-05-05", day.Travels.First().Departure.Date);
                Assert.Equal(LocationValues.California, day.Travels.First().Departure.Location);
                Assert.Equal("1200", day.Travels.First().Departure.Time);
                Assert.Equal("2011-05-05", day.Travels.First().Arrival.Date);
                Assert.Equal(LocationValues.China, day.Travels.First().Arrival.Location);
                Assert.Equal("1400", day.Travels.First().Arrival.Time);
                Assert.Equal("2011-05-05", day.Travels.Last().Departure.Date);
                Assert.Equal(LocationValues.China, day.Travels.Last().Departure.Location);
                Assert.Equal("1400", day.Travels.Last().Departure.Time);
                Assert.Equal("2011-05-05", day.Travels.Last().Arrival.Date);
                Assert.Equal(LocationValues.California, day.Travels.Last().Arrival.Location);
                Assert.Equal("1600", day.Travels.Last().Arrival.Time);
            });
        }
    }
}