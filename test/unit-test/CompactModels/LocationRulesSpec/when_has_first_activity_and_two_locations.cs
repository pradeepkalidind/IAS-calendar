using System;
using System.Collections.Generic;
using System.Linq;
using Calendar.General.Dto;
using Calendar.Model.Compact;
using Calendar.Service.Controller;
using Calendar.Service.Requests;
using Newtonsoft.Json;
using Xunit;

namespace Calendar.Tests.Unit.CompactModels.LocationRulesSpec
{
    [XUnitCases]
    public class when_has_first_activity_and_two_locations : CompactModelSpec
    {
        private static readonly Guid UserId = new Guid("C42D18E1-8B1F-43BD-8DCC-B75055F49AF1");
        private static CalendarController controller;

        [Fact]
        public void test_when_has_first_activity_and_two_locations()
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
                        FLA = 10,
                        FLD = 11,
                        SL = "CN/SH",
                        SLA = 11,
                        SLD = 12,
                        D = "2012/1/1"
                    }
                };

                controller.SaveDays(UserId.ToString(), new CalendarSaveDaysRequest { daysBody = JsonConvert.SerializeObject(calendarDayDtos) });
            });
            Then("should_be_save_with_empty_first_activity", () =>
            {
                var dayActivity = Session.Query<MonthActivity>().First().GetDay(1);
                Assert.Equal(DayType.ExtendedTrip, dayActivity.DayType);

            });
        }
    }
}