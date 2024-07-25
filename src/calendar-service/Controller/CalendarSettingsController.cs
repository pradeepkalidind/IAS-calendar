using System;
using Calendar.Service.Filters;
using Calendar.Service.Models;
using Calendar.Service.Requests;
using Calendar.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace Calendar.Service.Controller
{
    [UserBasicAuth]
    public class CalendarSettingsController : NewApiController
    {
        public ICalendarSettingsService Service;

        public CalendarSettingsController()
        {
            Service = new CalendarSettingsService(session);
        }

        private static Guid GetUserId(string userId)
        {
            return new Guid(userId);
        }

        [HttpPost("CalendarSettings/SaveWorkWeekDefaults")]
        public void SaveWorkWeekDefaults([FromForm] SaveWorkWeeksDefaultRequest request)
        {
            Service.SaveWorkWeekDefaults(GetUserId(request.Token), request.WorkWeekDefaults ?? Array.Empty<int>());
        }

        [HttpPost("CalendarSettings/SaveCalendarSettings")]
        public ActionResult SaveCalendarSettings([FromQuery] string token, [FromForm] SaveCalendarSettingsRequest request, [FromQuery] string culture = null)
        {
            var userId = GetUserId(token);
            Service.SaveWorkWeekDefaults(userId, request.WorkWeekDefaults ?? Array.Empty<int>());
            Service.SaveNationalHoliday(userId, request.Country ?? string.Empty);
            return Ok(Service.GetCalendarSettingsDto(userId, culture));
        }

        // This action is used for old Calendar UI, cannot be deleted.
        [HttpPost("CalendarSettings/GetCalendarSettings")]
        public ActionResult<DefaultWorkDaysDto> GetCalendarSettings([FromForm] GetCalendarSettingRequest request)
        {
            var userId = GetUserId(request.Token);
            return Ok(Service.GetCalendarSettings(userId));
        }

        [HttpPost("CalendarSettings/GetCalendarSettingsForNewUI")]
        public ActionResult GetCalendarSettingsForNewUi([FromQuery] string token, [FromQuery] string culture = null)
        {
            var userId = GetUserId(token);
            var result = Service.GetCalendarSettingsDto(userId, culture);
            return Ok(result);
        }
    }
}