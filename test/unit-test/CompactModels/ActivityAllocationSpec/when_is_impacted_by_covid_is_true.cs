using System;
using System.Collections.Generic;
using System.Linq;
using Calendar.General.Dto;
using Calendar.Model.Compact;
using Calendar.Service.Controller;
using Calendar.Service.Requests;
using Calendar.Tests.Unit.CompactModels.LocationRulesSpec;
using HttpContextMoq;
using HttpContextMoq.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Xunit;

namespace Calendar.Tests.Unit.CompactModels.ActivityAllocationSpec
{
    public class when_is_impacted_by_covid_is_true : CompactModelSpec
    {
        [Fact]
        public void should_change_activity_when_covid_is_true_and_date_before_2020_04_01()
        {
            var userId = Guid.NewGuid();
            var controller = new CalendarController();
            var calendarDayDtos = new List<CalendarDayDto>
            {
                new CalendarDayDto()
                {
                    D = "2019/3/31",
                    A = "CW",
                    SA = "CNW"
                },
                new CalendarDayDto()
                {
                    D = "2019/4/1",
                    A = "CW",
                    SA = "CNW"
                }
            };

            controller.SaveDays(userId.ToString(), new CalendarSaveDaysRequest
            {
                daysBody = JsonConvert.SerializeObject(calendarDayDtos)
            });

            Session.Clear();
            var march = Session.Query<MonthActivity>().First(m => m.Year == 2019 && m.Month == 3);
            var dayActivity1 = march.GetDay(31);

            Assert.Equal(ActivityType.Work, dayActivity1.First.Type);
            Assert.Equal(ActivityType.NonWork, dayActivity1.Second.Type);

            var april = Session.Query<MonthActivity>().First(m => m.Year == 2019 && m.Month == 4);
            var dayActivity2 = april.GetDay(1);

            Assert.Equal(ActivityType.COVID19Work, dayActivity2.First.Type);
            Assert.Equal(ActivityType.COVID19NonWork, dayActivity2.Second.Type);
        }

        [Fact]
        void should_change_activity_when_covid_is_true_and_date_before_04_01_when_user_save_from_journery_app()
        {
            var daysController = new DaysController
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new HttpContextMock().SetupUrl("http://localhost")
                        .SetupRequestHeaders(new Dictionary<string, StringValues>()
                        {
                            { "Application_Id", new StringValues("mobile_sync") }
                        })
                }
            };
            var userId = Guid.NewGuid();
            var updateDays = new ChangedDaysDto
            {
                deletedDays = new List<DeletedDayDto>(),
                updatedDays = new List<DayDto>
                {
                    new DayDto { firstActivity = "/ActivityType/Work", secondActivity = "/ActivityType/NonWork", date = "2019/03/31", isCovid = true },
                    new DayDto { firstActivity = "/ActivityType/Work", secondActivity = "/ActivityType/NonWork", date = "2019/04/01", isCovid = true },
                }
            };

            var jsonData = JsonConvert.SerializeObject(updateDays);
            daysController.SaveByUser(userId.ToString(), new SaveDaysRequest
            {
                Params = jsonData
            });

            var march = Session.Query<MonthActivity>().First(m => m.UserId == userId && m.Year == 2019 && m.Month == 3);
            Assert.Equal(ActivityType.Work, march.GetDay(31).First.Type);
            Assert.Equal(ActivityType.NonWork, march.GetDay(31).Second.Type);

            var april = Session.Query<MonthActivity>().First(m => m.UserId == userId && m.Year == 2019 && m.Month == 4);
            Assert.Equal(ActivityType.COVID19Work, april.GetDay(1).First.Type);
            Assert.Equal(ActivityType.COVID19NonWork, april.GetDay(1).Second.Type);
        }
    }
}
