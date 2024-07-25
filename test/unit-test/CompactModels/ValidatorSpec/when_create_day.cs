using System;
using Calendar.Model.Compact;
using Calendar.Tests.Unit.Services.Fixtures;
using Xunit;

namespace Calendar.Tests.Unit.CompactModels.ValidatorSpec
{
    [XUnitCases]
    public class when_create_day : PersistenceSpec
    {
        private static DayActivity GetUserDay(Action<DayActivityBuilder> changes)
        {
            new LocationPattern(LocationValues.China, 2, 4, LocationValues.UnitedState, 6, 8,
                LocationValues.UnitedKingdom, 10);

            var dayActivityBuilder = new DayActivityBuilder();
            dayActivityBuilder.FirstActivityType(ActivityType.Work)
                .FirstActivityFirstLocationAllocation(20)
                .SecondActivityType(ActivityType.Vacation)
                .SecondActivityFirstLocationAllocation(40)
                .Location(b => b
                    .FirstLocation(LocationValues.China)
                    .FirstLocationArrivalTime(2)
                    .FirstLocationDepartureTime(4)
                    .SecondLocation(LocationValues.UnitedState)
                    .SecondLocationArrivalTime(6)
                    .SecondLocationDepartureTime(8)
                    .ThirdLocation(LocationValues.UnitedKingdom)
                    .ThirdLocationArrivalTime(10)
                );
            changes(dayActivityBuilder);
            var day = dayActivityBuilder.Build();

            return day;
        }

        [Fact]
        public void test_when_create_day()
        {
            Then("should_be_invalid_if_first_location_is_invalid", () =>
            {
                var day = GetUserDay(d => d.Location(l => l.FirstLocation("invalid")));
                Assert.False(day.IsValid());
            });
            Then("should_be_invalid_if_second_location_is_invalid", () =>
            {
                var day = GetUserDay(d => d.Location(l => l.SecondLocation("invalid")));
                Assert.False(day.IsValid());
            });
            Then("should_be_invalid_if_third_location_is_invalid", () =>
            {
                var day = GetUserDay(d => d.Location(l => l.ThirdLocation("invalid")));
                Assert.False(day.IsValid());
            });
            Then("should_be_invalid_if_first_travel_departure_or_arrival_time_is_empty", () =>
            {
                Assert.False(GetUserDay(d => d.Location(l => l.FirstLocation(null))).IsValid());
                Assert.False(GetUserDay(d => d.Location(l => l.FirstLocationDepartureTime(null))).IsValid());
                Assert.False(GetUserDay(d => d.Location(l => l.SecondLocationArrivalTime(null))).IsValid());

                Assert.False(GetUserDay(d => d.Location(l => l.FirstLocationDepartureTime(null).SecondLocationArrivalTime(null))).IsValid());

            });
            Then("should_be_invalid_if_second_travel_departure_time_or_arrival_time_is_empty", () =>
            {
                Assert.False(GetUserDay(d => d.Location(l => l.SecondLocationDepartureTime(null))).IsValid());
                Assert.False(GetUserDay(d => d.Location(l => l.ThirdLocationArrivalTime(null))).IsValid());
            });
            Then("should_be_valid_if_first_travel_departure_or_arrival_time_is_not_empty", () =>
            {
                Assert.True(GetUserDay(d => { }).IsValid());
                Assert.True(GetUserDay(d => d.Location(l => l.SecondLocation(LocationPattern.Intransit)
                    .SecondLocationArrivalTime(null))).IsValid());
            });
        }
    }
}