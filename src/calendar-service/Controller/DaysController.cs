using System;
using System.Linq;
using Calendar.General.Dto;
using Calendar.General.Persistence;
using Calendar.Model.Convertor;
using Calendar.Models.Feed;
using Calendar.Persistence;
using Calendar.Service.Exceptions;
using Calendar.Service.Filters;
using Calendar.Service.Requests;
using Calendar.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace Calendar.Service.Controller
{
    [UserBasicAuth]
    public class DaysController : NewApiController
    {
        private readonly DayService service;
        private readonly ISessionWrapper oldSession;

        public DaysController()
        {
            service = new DayService(session);
            oldSession = DbInitializerFactory.Get().GetSession();
        }

        protected override void DisposeManagedResource()
        {
            oldSession?.Dispose();
        }

        [HttpGet("Days")]
        public ActionResult<ChangedDaysDto> Read(string userId, string fromDate, string toDate, string changedStartDate=null, string changedEndDate=null)
        {
            var userGuid = Guid.Parse(userId);
            var daysCriteria = GetCriteria(userId, fromDate, toDate);
            var calendarDtoRetriever = new CalendarDtoRetriever(session);
            ChangedDaysDto changedDaysDto;
            if (changedStartDate == null && changedEndDate == null)
            {
                changedDaysDto = calendarDtoRetriever.RetrieveDelta(userGuid, (DateTime)daysCriteria.FromDate, (DateTime)daysCriteria.ToDate);
            }
            else
            {
                var changedCriteria = GetCriteria(userId, changedStartDate ?? new DateTime(2010, 1, 1).ToShortDateString(), changedEndDate ?? DateTime.Today.AddDays(1).ToShortDateString());
                var period = new Period(ToUtc((DateTime)changedCriteria.FromDate), ToUtc((DateTime)changedCriteria.ToDate));
                var repo = new FeedEntryRepository(oldSession);
                var changedDays = new UserFeedCollector(period, session, repo).GetChangedDays(userGuid);
                var dates = changedDays.Where(d => d >= (DateTime)daysCriteria.FromDate && d <= (DateTime)daysCriteria.ToDate);
                changedDaysDto = calendarDtoRetriever.RetrieveChangedDays(userGuid, dates);
            }

            return Ok(changedDaysDto);
        }

        private static DateTime ToUtc(DateTime dateTime)
        {
            return new DateTime(dateTime.Ticks, DateTimeKind.Utc);
        }

        private static dynamic GetCriteria(string userId, string fromDate, string toDate)
        {
            try
            {
                return new
                {
                    UserId = Guid.Parse(userId),
                    FromDate = DateTime.Parse(fromDate),
                    ToDate = DateTime.Parse(toDate)
                };
            }
            catch (Exception)
            {
                throw new ArgumentException("invalid year month.");
            }
        }

        [HttpPost("User/{userId}/Days")]
        public ActionResult SaveByUser(string userId, [FromForm] SaveDaysRequest request)
        {
            string applicationId = null;
            var applicationHeader = Request.Headers["Application_Id"];
            if (applicationHeader.Count != 0)
            {
                applicationId = applicationHeader.First();
            }
            try
            {
                service.SaveOrDelete(request.Params, userId, applicationId);
            }
            catch (ArgumentException e)
            {
                throw new HttpException(400, (e.Message + request.Params).ToWeb());
            }

            return Ok();
        }
    }
    internal static class StringExtension
    {
        public static string ToWeb(this string str)
        {
            return str.Replace(Environment.NewLine, "<br/>");
        }
    }
}