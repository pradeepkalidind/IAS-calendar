using System;
using System.Collections.Generic;
using System.Linq;
using Calendar.General.Dto;
using Calendar.Model.Compact;
using Calendar.Service.Controller;
using Calendar.Service.Requests;
using Calendar.Tests.Unit.Services.Fixtures;
using Newtonsoft.Json;
using Xunit;

namespace Calendar.Tests.Unit.CompactModels.LocationRulesSpec
{
    [XUnitCases]
    public class when_second_location_is_blank : CompactModelSpec
    {
        private static readonly Guid UserId = new Guid("C42D18E1-8B1F-43BD-8DCC-B75055F49AF1");
        private static CalendarController controller;

        [Fact]
        public void test_when_second_location_is_blank()
        {
            Given("context", () =>
            {
                controller = new CalendarController();

                var calendarDayDtos = new List<CalendarDayDto>
                {
                    new CalendarDayDto()
                    {
                        A = "W",
                        FL = "CN/BJ",
                        SLA = 10,
                        SLD = 11,
                        TL = "US/NY",
                        TLA = 12,
                        D = "2012/1/1"
                    }
                };
                controller.SaveDays(UserId.ToString(), new CalendarSaveDaysRequest { daysBody = JsonConvert.SerializeObject(calendarDayDtos) });
            });
            Then("should_be_save_with_empty_second_location", () =>
            {
                var dayActivity = Session.Query<MonthActivity>().First().GetDay(1);
                Assert.Equal(new LocationPattern(LocationValues.Beijing), dayActivity.LocationPattern);
                Assert.Equal(DayType.Standard, dayActivity.DayType);
            });
        }
    }
}