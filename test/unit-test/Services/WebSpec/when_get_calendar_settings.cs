using System;
using System.Linq;
using Calendar.General.Persistence;
using Calendar.Model.Compact;
using Calendar.Service.Controller;
using Calendar.Service.Models;
using Calendar.Service.Requests;
using Calendar.Service.Services;
using Calendar.Tests.Unit.CompactModels.LocationRulesSpec;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Calendar.Tests.Unit.Services.WebSpec
{
    [Collection("NotSharedDatabaseCollection")]
    public class when_get_calendar_settings : CompactModelSpec
    {
        private static CalendarSettingsController controller;
        private static DefaultWorkDays model;
        private static Guid userId;
        private static Mock<INationalHolidayService> holidayService;
        private static ICalendarSettingsService calendarSettingsService;

        public when_get_calendar_settings()
        {
            holidayService = new Mock<INationalHolidayService>();
            calendarSettingsService = new CalendarSettingsService(Session)
            {
                holidayService = holidayService.Object
            };

            controller = new CalendarSettingsController()
            {
                Service = calendarSettingsService
            };
            userId = Guid.NewGuid();
            model =
                new DefaultWorkDays
                {
                    UserId = userId,
                    Days = 41 // 0101001
                };
            Session.SaveOrUpdateAndFlush(model);
        }

        [Fact]
        public void should_get_calendar_settings()
        {
            var resp = controller.GetCalendarSettings(new GetCalendarSettingRequest
            {
                Token = userId.ToString()
            });

            var serializedResult = ((OkObjectResult)resp.Result)?.Value as DefaultWorkDaysDto;
            var weekWorkDays = serializedResult?.wwd;

            Assert.NotNull(weekWorkDays);
            Assert.Equal(3, weekWorkDays.Count);
            Assert.Equal(2, weekWorkDays.ElementAt(0));
            Assert.Equal(4, weekWorkDays.ElementAt(1));
            Assert.Equal(0, weekWorkDays.ElementAt(2));
        }
    }
}
