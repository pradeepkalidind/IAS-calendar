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
    public class when_retrieve_all_calendar_data_of_user : DbSpec
    {
        private static readonly Guid UserId = Guid.NewGuid();
        private static ISessionWrapper session;
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
            var dayActivity = new DayActivity(new Model.Compact.Activity(ActivityType.Work, 20),
                new Model.Compact.Activity(ActivityType.Sick, 30),
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
        public void test_when_retrieve_all_calendar_data_of_user()
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
                Assert.Equal(Calendar.Models.ActivityType.Work, day.Activities.First().Type);
                Assert.Equal(0.2, day.Activities.First().FirstAllocation);
                Assert.Equal(0.8, day.Activities.First().SecondAllocation);
                Assert.Equal(Calendar.Models.ActivityType.Sick, day.Activities.Last().Type);
                Assert.Equal(0.3, day.Activities.Last().FirstAllocation);
                Assert.Equal(0.7, day.Activities.Last().SecondAllocation);

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

    public class when_retrieve_all_calendar_with_covid : DbSpec
    {
        private readonly ISessionWrapper session;
        private readonly Guid UserId = Guid.NewGuid();
        private UserCalendar UserCalendar;

        public when_retrieve_all_calendar_with_covid()
        {
            session = DbHelper.GetSession();
        }

        [Fact]
        public void should_return_day_with_covid()
        {
            var monthActivity = new MonthActivity(UserId, 2019, 4);
            var location = new LocationPattern(LocationValues.China);
            var errMsg = new List<string>();
            monthActivity.Update(new DayActivity(new Model.Compact.Activity(ActivityType.COVID19Work, 100), new Model.Compact.Activity(ActivityType.COVID19NonWork, 0), location, 1,
                errMsg));
            monthActivity.Update(new DayActivity(new Model.Compact.Activity(ActivityType.Work, 100), new Model.Compact.Activity(ActivityType.NonWork, 0), location, 2, errMsg));
            session.SaveOrUpdateAndFlush(monthActivity);

            UserCalendar = new CalendarDtoRetriever(session).Retrieve(UserId);

            Assert.Equal("TRUE", UserCalendar.Days.Single(d => d.Date == "2019-04-01").IsCovid);
            Assert.Equal("FALSE", UserCalendar.Days.Single(d => d.Date == "2019-04-02").IsCovid);

            Cleaner.CleanTable(session, typeof(MonthActivity));
        }
    }
}