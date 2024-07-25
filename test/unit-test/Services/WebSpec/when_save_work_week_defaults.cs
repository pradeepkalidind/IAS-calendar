using System;
using System.Linq;
using Calendar.General.Persistence;
using Calendar.Model.Compact;
using Calendar.Service.Controller;
using Calendar.Service.Requests;
using Calendar.Service.Services;
using Calendar.Tests.Unit.CompactModels.LocationRulesSpec;
using Moq;
using Xunit;

namespace Calendar.Tests.Unit.Services.WebSpec
{
    [Collection("NotSharedDatabaseCollection")]
    public class when_save_work_week_defaults : CompactModelSpec
    {
        private static CalendarSettingsController controller;
        private static Guid userId;

        public when_save_work_week_defaults()
        {
            var holidayService = new Mock<INationalHolidayService>();
            var settingService = new CalendarSettingsService(Session)
            {
                holidayService = holidayService.Object
            };
            controller = new CalendarSettingsController
            {
                Service = settingService
            };
            userId = Guid.NewGuid();
            var model = new DefaultWorkDays { UserId = userId, Days = 41 /*0101001*/ };
            Session.SaveOrUpdateAndFlush(model);
        }

        [Fact]
        public void should_success()
        {
            controller.SaveWorkWeekDefaults(new SaveWorkWeeksDefaultRequest
            {
                Token = userId.ToString(),
                WorkWeekDefaults = new[] { 4, 5, 0 }
            }); // = 0001101 = 13
            var settings = DbConfigurationFactory.Get().GetSession().Query<DefaultWorkDays>()
                .First(d => d.UserId.Equals(userId));

            Assert.Equal(13, settings.Days);
        }
    }
}