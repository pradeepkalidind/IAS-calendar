using System;
using System.Linq;
using Calendar.Model.Compact;
using Calendar.Service.Services;
using Calendar.Tests.Unit.CompactModels.LocationRulesSpec;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Calendar.Tests.Unit.Services.WebSpec
{
    [Collection("NotSharedDatabaseCollection")]
    public class when_save_new_default_work_days : CompactModelSpec
    {
        private static Guid userId;
        private static CalendarSettingsService service;

        public when_save_new_default_work_days()
        {
            userId = Guid.NewGuid();
            var holidayService = new Mock<INationalHolidayService>();
            service = new CalendarSettingsService(Session)
            {
                holidayService = holidayService.Object
            };
            var json = JsonConvert.SerializeObject(
                new DefaultWorkDays
                {
                    UserId = userId,
                    Days = 1
                });
            service.CreateOrUpdate(json, userId.ToString());
        }

        [Fact]
        public void should_get_the_created_default_work_days()
        {
            var defaultWorkingDaysForMobile = service.GetDefaultWorkingDaysForMobile(userId);

            Assert.Single(defaultWorkingDaysForMobile);

            var defaultWorkDaysDto = defaultWorkingDaysForMobile.ElementAt(0);

            Assert.Equal(userId, defaultWorkDaysDto.userId);
            Assert.Equal((byte)1, defaultWorkDaysDto.days);
        }
    }
}
