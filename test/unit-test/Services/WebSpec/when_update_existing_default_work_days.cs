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
    public class when_update_existing_default_work_days : CompactModelSpec
    {
        private static Guid userId;
        private static CalendarSettingsService service;

        public when_update_existing_default_work_days()
        {
            userId = Guid.NewGuid();
            service = new CalendarSettingsService(Session)
            {
                holidayService = new Mock<INationalHolidayService>().Object
            };
            var model = new DefaultWorkDays
            {
                UserId = userId,
                Days = 7
            };
            Session.Save(model);
            var json = JsonConvert.SerializeObject(
                new DefaultWorkDays
                {
                    UserId = userId,
                    Days = 12
                });
            service.CreateOrUpdate(json, userId.ToString());
        }

        [Fact]
        public void should_get_the_updated_default_work_days()
        {
            var defaultWorkingDaysForMobile = service.GetDefaultWorkingDaysForMobile(userId);
            var defaultWorkDaysDto = defaultWorkingDaysForMobile.ElementAt(0);

            Assert.Equal(userId, defaultWorkDaysDto.userId);
            Assert.Equal((byte)12, defaultWorkDaysDto.days);
        }
    }
}
