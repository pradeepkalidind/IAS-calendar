using System;
using System.Linq;
using Calendar.Model.Compact;
using Calendar.Service.Services;
using Calendar.Tests.Unit.CompactModels.LocationRulesSpec;
using Moq;
using Xunit;

namespace Calendar.Tests.Unit.Services.WebSpec
{
    [Collection("NotSharedDatabaseCollection")]
    public class when_save_and_update_national_holiday_settings : CompactModelSpec
    {
        private static Guid userId;
        private static CalendarSettingsService service;

        private static string country;

        public when_save_and_update_national_holiday_settings()
        {
            var holidayService = new Mock<INationalHolidayService>();
            userId = Guid.NewGuid();
            country = "USA";
            service = new CalendarSettingsService(Session)
            {
                holidayService = holidayService.Object
            };
        }

        [Fact]
        public void should_convert_dto_to_correct_object()
        {
            service.SaveNationalHoliday(userId, country);
            service.SaveNationalHoliday(userId, country);
            service.SaveNationalHoliday(userId, country);
            var records = Session.Query<NationalHolidaySetting>().Where(setting => setting.UserId == userId).ToArray();

            Assert.Single(records);
            Assert.Equal(userId, records[0].UserId);
            Assert.Equal(country, records[0].Country);
        }
    }
}
