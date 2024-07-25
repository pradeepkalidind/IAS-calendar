using System;
using System.Collections.Generic;
using Calendar.Model.Compact;

namespace Calendar.Service.Models
{
    public class DefaultWorkDaysDto
    {
        public IList<int> wwd;
    }

    public static class CalendarSettingsExtensions
    {
        public static DefaultWorkDaysDto ToCalendarSettingDto(this DefaultWorkDays defaultWorkDays)
        {
            var weekWorkDays = new List<int>();
            var daysArray = string.Format("{0,7}", Convert.ToString(defaultWorkDays.Days, 2));
            for (var i = 0; i < 7; i++)
            {
                if (daysArray[i] == '1')
                {
                    if (i == 6)
                    {
                        weekWorkDays.Add(0);
                        continue;
                    }
                    weekWorkDays.Add(i + 1);
                }
            }
            return new DefaultWorkDaysDto
            {
                wwd = weekWorkDays
            };
        }
    }
}