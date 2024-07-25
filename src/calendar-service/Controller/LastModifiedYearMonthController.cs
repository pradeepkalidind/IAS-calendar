using System;
using Calendar.Model.Compact;
using Calendar.Service.Filters;
using Calendar.Service.Requests;
using Calendar.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace Calendar.Service.Controller
{
    [UserBasicAuth]
    public class LastModifiedYearMonthController : NewApiController
    {
        private readonly LastModifiedYearMonthService service;

        public LastModifiedYearMonthController()
        {
            service = new LastModifiedYearMonthService(session);
        }

        [HttpGet("User/{userId}/LastModifiedYearMonth")]
        public ActionResult<LastModifiedYearMonth> Get(Guid userId)
        {
            var lastModifiedYearMonth = service.GetByUser(userId);

            if (lastModifiedYearMonth == null)
            {
                return Ok();
            }

            return Ok(lastModifiedYearMonth);
        }

        [HttpPost("User/{userId}/LastModifiedYearMonth")]
        public ActionResult Set(Guid userId, [FromForm] UpdateLastModifiedYearMonthRequest request)
        {
            if (!IsMonthValid(request.Month) || !IsYearValid(request.Year))
            {
                return BadRequest("Invalid year or month.");
            }

            service.SetLastModifiedMonth(userId, request.Year, request.Month);
            return Ok();
        }

        private bool IsYearValid(int year)
        {
            return year > 0;
        }

        private bool IsMonthValid(int month)
        {
            return month > 0 && month < 13;
        }
    }
}
