using System;
using Calendar.General.Dto;
using Calendar.Service.Filters;
using Calendar.Service.Requests;
using Calendar.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace Calendar.Service.Controller
{
    [UserBasicAuth]
    public class DefaultWorkDaysController : NewApiController
    {
        private readonly CalendarSettingsService service;

        public DefaultWorkDaysController()
        {
            service = new CalendarSettingsService(session);
        }

        [HttpGet("User/{userId}/DefaultWorkDays")]
        public ActionResult<DefaultWorkDaysDto> Read(Guid userId)
        {
            return Ok(service.GetDefaultWorkingDaysForMobile(userId));
        }

        [HttpPost("User/{userId}/DefaultWorkDays")]
        public ActionResult SaveByUser([FromRoute] string userId, [FromForm] SaveWorkDaysRequest request)
        {
            try
            {
                service.CreateOrUpdate(request.Params, userId);
                return Ok();
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message + request.Params);
            }
        }
    }
}