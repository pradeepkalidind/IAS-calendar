using System;
using Calendar.Client.Schema;
using Calendar.Model.Compact.Archive.Rule;
using Calendar.Model.Convertor;
using Microsoft.AspNetCore.Mvc;
using TableExporter.Services;

namespace Calendar.Service.Controller
{
    public class ArchiveAssigneeController : NewApiController
    {
        [HttpGet("archive-assignees/{iasPlatformUserId}")]
        public ActionResult<TableInfoDto> Get(string iasPlatformUserId)
        {
            var userId = new Guid(iasPlatformUserId);
            var calendarDto = new CalendarDtoRetriever(session).Retrieve(userId);

            return Ok(new TableInfoDto
            {
                All = new CalendarInfoDto
                {
                    CalendarInfo = calendarDto
                },
                NonPii = new CalendarInfoDto
                {
                    CalendarInfo = calendarDto
                }
            });
        }

        [HttpDelete("archive-assignees/{iasPlatformUserId}")]
        public ActionResult Delete(string iasPlatformUserId)
        {
            var tableDeleteService = new TableDeleteServiceV2();
            var userId = new Guid(iasPlatformUserId);

            tableDeleteService.Delete<UserActivityRule>(session.Session, userId);
            tableDeleteService.Delete<DefaultWorkDaysRule>(session.Session, userId);
            tableDeleteService.Delete<NationalHolidaySettingRule>(session.Session, userId);
            tableDeleteService.Delete<MonthActivityRule>(session.Session, userId);
            tableDeleteService.Delete<NoteRule>(session.Session, userId);

            return Ok((object)new { });
        }
    }
}