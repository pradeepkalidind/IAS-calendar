using System;

namespace Calendar.Models.Feed
{
    public class Period
    {
        public Period(DateTime startTime, DateTime endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}