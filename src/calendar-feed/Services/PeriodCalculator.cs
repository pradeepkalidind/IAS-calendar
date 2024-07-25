using System;
using Calendar.Models.Feed;

namespace Calendar.Feed.Services
{
    public static class PeriodCalculator
    {
        private static readonly DateTime BaseTime = new DateTime(2011, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static Period Calculate(DateTime time, int duration)
        {
            // Using standardTime to ensure time accuracy. Zhang Yue
            var standardTime = new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, 0, 0, DateTimeKind.Utc);
            var timeSpan = (standardTime - BaseTime).TotalMinutes;
            var startTime = standardTime.AddMinutes(-timeSpan % duration);
            return new Period(startTime, startTime.AddMinutes(duration));
        }
    }
}