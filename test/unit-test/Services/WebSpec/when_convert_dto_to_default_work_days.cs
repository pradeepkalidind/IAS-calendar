using System;
using System.Linq;
using Calendar.General.Persistence;
using Calendar.Model.Compact;
using Calendar.Service.Services;
using Calendar.Tests.Unit.CompactModels.LocationRulesSpec;
using Moq;
using Xunit;

namespace Calendar.Tests.Unit.Services.WebSpec
{
    [Collection("NotSharedDatabaseCollection")]
    public class when_convert_dto_to_default_work_days : CompactModelSpec
    {
        private static Guid userId;
        private static CalendarSettingsService service;
        private static DefaultWorkDays model;

        public when_convert_dto_to_default_work_days()
        {
            userId = Guid.NewGuid();
            model =
                new DefaultWorkDays
                {
                    UserId = userId,
                    Days = 41 // = 0101001
                };

            Session.CreateAndFlush(model);

            var holidayService = new Mock<INationalHolidayService>();

            service = new CalendarSettingsService(Session)
            {
                holidayService = holidayService.Object
            };
        }

        [Fact]
        public void should_covert_dto_to_correct_object()
        {
            service.SaveWorkWeekDefaults(userId, new[] { 4, 5, 0 }); // = 0001101 = 13

            var settings = Session.Query<DefaultWorkDays>().First(d => d.UserId.Equals(userId));

            Assert.Equal((byte)13, settings.Days);
        }
    }
}
