using System;
using System.Collections.Generic;

namespace Calendar.Persistence
{
    public class MonthSplitter
    {
        public static IEnumerable<DateTime> FindDaysInPeriod(DateTime start, DateTime end, Guid userId, Func<DateTime, DateTime, Guid, IEnumerable<DateTime>> getDays)
        {

            var result = new List<DateTime>();
            var startTime = start;
            while (true)
            {
                var nextMonth = startTime.AddMonths(1);
                var nextMonthStart = new DateTime(nextMonth.Year, nextMonth.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                if (end <= nextMonthStart)
                {
                    result.AddRange(getDays(startTime, end, userId));
                    break;
                }
                result.AddRange(getDays(startTime, nextMonthStart, userId));
                startTime = nextMonthStart;
            }
            return result;
        }
    }
}
