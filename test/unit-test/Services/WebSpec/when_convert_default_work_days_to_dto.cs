using System;
using Calendar.General.Persistence;
using Calendar.Model.Compact;
using Calendar.Service.Services;
using Calendar.Tests.Unit.CompactModels.LocationRulesSpec;
using Moq;
using Xunit;

namespace Calendar.Tests.Unit.Services.WebSpec
{
    [Collection("NotSharedDatabaseCollection")]
    public class when_convert_default_work_days_to_dto : CompactModelSpec
    {
        private static Guid userId;
        private static CalendarSettingsService service;
        private static DefaultWorkDays model;

        [Fact]
        public void should_covert_to_correct_dto()
        {
            userId = Guid.NewGuid();
            model =
                new DefaultWorkDays
                {
                    UserId = userId,
                    Days = 41 // 0101001
                };
            Session.CreateAndFlush(model);
            var holidayService = new Mock<INationalHolidayService>();
            service = new CalendarSettingsService(Session)
            {
                holidayService = holidayService.Object
            };

            var calendarSettingsDto = service.GetDefaultWorkDays(userId);

            var weekWorkDays = calendarSettingsDto.wwd;

            Assert.Equal(3, weekWorkDays.Count);
            Assert.Equal(2, weekWorkDays[0]);
            Assert.Equal(4, weekWorkDays[1]);
            Assert.Equal(0, weekWorkDays[2]);
        }
    }
}