using System;
using System.Collections.Generic;
using System.Linq;

namespace Calendar.Client.Schema
{
    public static class UserCalendarExtensions
    {
        public static Day FindOrCreate(this UserCalendar userCalendar, DateTime date)
        {
            var dateStr = date.ToString("yyyy-MM-dd");
            var day = userCalendar.Days.Where(d => d.Date.Equals(dateStr)).FirstOrDefault() ?? new Day { Date = dateStr };
            userCalendar.Days.Add(day);
            return day;
        }

        public static void SortDays(this UserCalendar userCalendar)
        {
            var days = userCalendar.Days.OrderBy(d => DateTime.Parse(d.Date));
            userCalendar.Days = new HashSet<Day>();
            foreach (var day in days)
            {
                userCalendar.Days.Add(day);
            }
        }
    }
}