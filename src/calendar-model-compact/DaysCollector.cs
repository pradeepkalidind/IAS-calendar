using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Calendar.Persistence;
using NHibernate;

namespace Calendar.Model.Compact
{
    public class DaysCollector
    {
        private readonly Guid userId;
        private readonly ISessionWrapper session;
        
        private MonthActivity[] monthActivitiesForMissingTravel;
        private MonthActivity[] monthActivities;

        public DaysCollector(Guid userId, ISessionWrapper session)
        {
            this.userId = userId;
            this.session = session;
        }

        private void Collect(DateTime startDate, DateTime endDate)
        {
            var endTime = endDate.AddDays(1);
            monthActivitiesForMissingTravel = session.Query<MonthActivity>().Where(Expression(startDate,endTime, userId)).ToArray();
            monthActivities = monthActivitiesForMissingTravel.Where(Expression(startDate,endDate, userId).Compile()).ToArray();
        }

        public IEnumerable<MonthActivity> GetMonthActivities(DateTime startDate, DateTime endDate)
        {
            return session.Query<MonthActivity>().Where(Expression(startDate, endDate, userId)).ToArray();
        }

        public bool IsCompleted(DateTime startDate, DateTime endDate)
        {
            var totalDays = (int)(endDate.AddDays(1) - startDate).TotalDays;
            Collect(startDate,endDate);
            var completeDays = GetCompleteDays(startDate,endDate);
            return completeDays == totalDays;
        }

        public int GetCompleteDays(DateTime startDate, DateTime endDate)
        {
            Collect(startDate, endDate);
            return monthActivities.Sum(m => GetCompleteDaysOfMonth(m, startDate, endDate));
        }

        private int GetCompleteDaysOfMonth(MonthActivity monthActivity, DateTime startDate, DateTime endDate)
        {
            var dayActivities = BuildDayActivities(monthActivity, startDate, endDate);
            return dayActivities.Count(d => d.IsComplete(today => FindNextDayIn(monthActivity, today)));
        }

        private int GetCompleteDaysOfMonth(MonthActivity monthActivity)
        {
            var dayActivities = monthActivity.GetDays();
            return dayActivities.Count(d => d.IsComplete(today => FindNextDayIn(monthActivity, today)));
        }

        public static IEnumerable<DayActivity> BuildDayActivities(MonthActivity monthActivity, DateTime startDate, DateTime endDate)
        {
            var days = GetDays(monthActivity, startDate, endDate);
            return monthActivity.GetDays(days);
        }

        private static IEnumerable<int> GetDays(MonthActivity monthActivity, DateTime startDate, DateTime endDate)
        {
            var startDay = (monthActivity.Year == startDate.Year && monthActivity.Month == startDate.Month) ? startDate.Day : 1;
            var endDay = (monthActivity.Year == endDate.Year && monthActivity.Month == endDate.Month) ? endDate.Day : DateTime.DaysInMonth(monthActivity.Year, monthActivity.Month);
            return Enumerable.Range(startDay, endDay - startDay + 1);
        }

        public static IEnumerable<DayActivity> BuildDayActivities(MonthActivity monthActivity, IEnumerable<DateTime> dates)
        {
            var days = dates.Where(d=>d.Year == monthActivity.Year && d.Month == monthActivity.Month).Select(d=>d.Day).ToArray();
            return monthActivity.GetDays(days);
        }

        private DayActivity FindNextDayIn(MonthActivity monthActivity, DayActivity today)
        {
            if (today.Day == DateTime.DaysInMonth(monthActivity.Year, monthActivity.Month))
            {
                var nextMonth = monthActivitiesForMissingTravel.FirstOrDefault(m => m.Year == monthActivity.Year && m.Month == monthActivity.Month + 1);
                return nextMonth == null ? null : nextMonth.GetDay(1);
            }
            return monthActivity.GetDay(today.Day + 1);
        } 

        public string GetCompleteDaysByMonth(DateTime startDate, DateTime endDate)
        {
            var daysByMonth = new List<int>();
            startDate = new DateTime(startDate.Year, startDate.Month, 1);
            endDate = new DateTime(endDate.Year, endDate.Month, DateTime.DaysInMonth(endDate.Year, endDate.Month));
            Collect(startDate, endDate);

            for (var i = new DateTime(startDate.Year, startDate.Month, 1); i <= endDate; i = i.AddMonths(1))
            {
                var month = i.Month;
                var year = i.Year;
                var monthActivity = monthActivities.FirstOrDefault(m => m.Year == year && m.Month == month);
                daysByMonth.Add(monthActivity == null ? 0 : GetCompleteDaysOfMonth(monthActivity));
            }
            return string.Join(",", daysByMonth);
        }

        private static Expression<Func<MonthActivity, bool>> Expression(DateTime startDate, DateTime endTime, Guid userId)
        {
            return m => m.UserId == userId && (m.Year > startDate.Year || (m.Year == startDate.Year && m.Month >= startDate.Month)) &&
                (m.Year < endTime.Year || (m.Year == endTime.Year && m.Month <= endTime.Month));
        }
    }
}