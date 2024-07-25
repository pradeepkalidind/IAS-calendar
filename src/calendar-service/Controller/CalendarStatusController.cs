using System;
using System.Text;
using Calendar.Model.Compact;
using Calendar.Service.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Calendar.Service.Controller
{
    [UserBasicAuth]
    public class CalendarStatusController : NewApiController
    {
        [HttpGet("CalendarStatus/IsCompleted")]
        public ActionResult<bool> IsCompleted(string userId, int startYear, int startMonth, int endYear, int endMonth)
        {
            var criteria = GetCriteria(userId, startYear, startMonth, 1, endYear, endMonth, DateTime.DaysInMonth(endYear, endMonth));
            return Ok(new DaysCollector(criteria.UserId,session).IsCompleted(criteria.StartDate, criteria.EndDate));
        }

        [HttpGet("CalendarStatus/GetCompleteDays")]
        public ActionResult<int> GetCompleteDays(string userId, int startYear, int startMonth, int startDay, int endYear, int endMonth, int endDay)
        {
            var criteria = GetCriteria(userId, startYear, startMonth, startDay, endYear, endMonth, endDay);
            return Ok(new DaysCollector(criteria.UserId,session).GetCompleteDays(criteria.StartDate, criteria.EndDate));
        }

        [HttpGet("CalendarStatus/GetCompleteDaysByMonth")]
        public ActionResult GetCompleteDaysByMonth(string userId, int startYear, int startMonth, int startDay, int endYear, int endMonth, int endDay)
        {
            var criteria = GetCriteria(userId, startYear, startMonth, startDay, endYear, endMonth, endDay);
            var completeDaysByMonth = new DaysCollector(criteria.UserId, session).GetCompleteDaysByMonth(criteria.StartDate,
                criteria.EndDate);
            return Content(completeDaysByMonth, "text/plain", Encoding.UTF8);
        }
        
        private class Criteria
        {
            public Guid UserId { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }

        private Criteria GetCriteria(string userId, int startYear, int startMonth, int startDay, int endYear, int endMonth, int endDay)
        {
            try
            {
                return new Criteria
                {
                    UserId = Guid.Parse(userId),
                    StartDate = new DateTime(startYear, startMonth, startDay),
                    EndDate = new DateTime(endYear, endMonth, endDay)
                };
            }
            catch (Exception)
            {
                var stringBuilder = new StringBuilder("invalid year month: ");
                stringBuilder.AppendLine(string.Format("start: {0}-{1}-{2}", startYear, startMonth, startDay));
                stringBuilder.AppendLine(string.Format("end: {0}-{1}-{2}", endYear, endMonth, endDay));
                throw new ArgumentException(stringBuilder.ToString());
            }
        }
    }
}