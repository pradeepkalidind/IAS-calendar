using System;
using System.Collections.Generic;
using System.Text;
using Calendar.Client.Schema;
using Calendar.General.Models;
using Calendar.General.Persistence;
using Calendar.Model.Convertor;
using Calendar.Models.Feed;
using Calendar.Persistence;
using Calendar.Service.Filters;
using Calendar.Service.Formatters;
using Calendar.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Calendar.Service.Controller
{
    [UserBasicAuth]
    public class UserCalendarController : NewApiController
    {
        private readonly ISessionWrapper oldSession;

        public UserCalendarController()
        {
            oldSession = DbInitializerFactory.Get().GetSession();
        }

        protected override void DisposeManagedResource()
        {
            oldSession?.Dispose();
        }

        [HttpGet]
        [HttpPost]
        [Route("Calendars")]
        public ActionResult CrossUserCalendar(string userDayPairs)
        {
            var calendarDto = new UserDaysMappingExtractor(session).Retrieve(userDayPairs);
            return Content(CalendarFormatter.Format(calendarDto), CalendarFormatter.CalendarMediaType, Encoding.UTF8);
        }

        [HttpGet("IASPlatformUser/{iasPlatformUserId}/Calendar")]
        public ActionResult CalendarAll(string iasPlatformUserId)
        {
            var userId = new Guid(iasPlatformUserId);
            var calendarDto = new CalendarDtoRetriever(session).Retrieve(userId);
            return Content(CalendarFormatter.Format(calendarDto), CalendarFormatter.CalendarMediaType, Encoding.UTF8);
        }

        [HttpGet("IASPlatformUser/{iasPlatformUserId}/Calendar/{fromDate}/{toDate}")]
        public ActionResult Calendar(string iasPlatformUserId, string fromDate, string toDate)
        {
            var userId = new Guid(iasPlatformUserId);
            var start = DateTime.Parse(fromDate);
            var end = DateTime.Parse(toDate);
            var calendarDto = new CalendarDtoRetriever(session).Retrieve(userId, start, end);
            return Content(CalendarFormatter.Format(calendarDto), CalendarFormatter.CalendarMediaType, Encoding.UTF8);
        }

        [HttpGet("IASPlatformUser/{iasPlatformUserId}/CalendarByChange/{changedStart}/{changedEnd}")]
        public ActionResult CalendarByChange(string iasPlatformUserId, long changedStart, long changedEnd)
        {
            var userId = new Guid(iasPlatformUserId);
            var period = new Period(new DateTime(changedStart, DateTimeKind.Utc), new DateTime(changedEnd, DateTimeKind.Utc));
            var repo = new FeedEntryRepository(oldSession);

            var changedDays = new UserFeedCollector(period, session, repo).GetChangedDays(userId);
            var userCalendar = new CalendarDtoRetriever(session).Retrieve(userId, changedDays);
            var userCalendarWithDeleted = UserCalendarWithDeleted.Build(userCalendar, changedDays);
            return Content(CalendarFormatter.Format(userCalendarWithDeleted), CalendarFormatter.CalendarMediaType, Encoding.UTF8);
        }
    }

    public class UserDaysMappingExtractor
    {
        private readonly ISessionWrapper session;
        private Dictionary<Guid, List<DateTime>> userDayPairsUsingNew;

        public UserDaysMappingExtractor(ISessionWrapper session)
        {
            this.session = session;
        }

        public CalendarRoot Retrieve(string userDayPairs)
        {
            Extract(userDayPairs);
            var calendarDto = userDayPairsUsingNew.Count == 0
                ? new CalendarRoot()
                : new CalendarDtoRetriever(session).Retrieve(userDayPairsUsingNew);

            return calendarDto;
        }

        private void Extract(string userDayPairs)
        {
            var userDaysMappings = JsonConvert.DeserializeObject<List<UserAndDayPair>>(userDayPairs)
                .ToUserDaysMapping();
            userDayPairsUsingNew = new Dictionary<Guid, List<DateTime>>();

            foreach (var userDaysMapping in userDaysMappings)
            {
                userDayPairsUsingNew.Add(userDaysMapping.Key, userDaysMapping.Value);
            }
        }
    }
}